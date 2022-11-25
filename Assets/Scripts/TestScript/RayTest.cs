using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;
using DebugUtil;

using Unity.Mathematics;

namespace KWY
{
    public class RayTest : MonoBehaviour
    {
        [SerializeField]
        TilemapControl tilemapcontrol;
        [SerializeField]
        Tilemap map;

        private int layerMask;
        RaycastHit2D[] hits;
        private List<Vector2> correction = new List<Vector2>();
        private List<Vector2> direction = new List<Vector2>();
        private Vector2 lastPos;
        private float xCor = 0.65f * 1.5f;  //��1 -> ��2 x�� ����ġ
        private float yCor = 0.7f * 1.5f / 0.65f;  // ��1 -> ��2 y�� ����ġ

        public void Ray(Vector2 basePos, SkillBase sb, int dir, bool reversed)
        {
            Vector2 bp; // ��ġ ������ ����
            Vector2 dp; // ���� ����
            float d;    // ��Ÿ�

            if (reversed)
            {
                // ������ ���
                bp = basePos + correction[5 - dir];
                dp = direction[5 - dir];
            }
            else
            {
                // �ƴ� ���
                bp = basePos + correction[dir];
                dp = direction[dir];
            }

            //// ��Ÿ�
            //if (dir == 1 || dir == 4)
            //{
            //    d = sb.distance[dir];
            //}
            //else
            //{
            //    d = (sb.distance[dir]) / math.sqrt(2);
            //    d = sb.distance[dir];
            //}

            // ��Ÿ�
            d = sb.distance[dir];

            Debug.DrawRay(bp, dp * d, Color.blue, 2f);
            hits = Physics2D.RaycastAll(bp, dp, d, layerMask);

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                hit.transform.GetComponent<SpriteRenderer>().color = Color.red;
                Debug.Log(hit.transform.name);
            }
        }

        public void MultipleRay(Vector2 basePos, SkillBase sb, List<Direction> dir, bool reversed, int rays)
        {
            if (reversed)
            {
                for (int i = 0; i < rays; i++)
                {
                    Ray(basePos, sb, (int)dir[i], true);
                }
            }
            else
            {
                for (int i = 0; i < rays; i++)
                {
                    Ray(basePos, sb, (int)dir[i], false);
                }
            }
        }

        public void CurvedRay(Vector2 basePos, SkillBase sb, List<Direction> dir, int num, int overdir, bool team)
        {
            Vector2 bp;
            Vector2 dp;
            float d;

            if (num == 0)
            {
                lastPos = basePos;
            }

            if ((int)dir[num] == 6)
            {
                lastPos = basePos;
                return;
            }

            //// ������ ���
            //if (reversed)
            //{
            //    bp = lastPos + correction[5 - (int)dir[num]];
            //    dp = direction[5 - (int)dir[num]];
            //}
            //// �ƴ� ���
            //else
            //{
            //    bp = lastPos + correction[(int)dir[num]];
            //    dp = direction[(int)dir[num]];
            //}

            if(overdir == 0)
            {
                bp = lastPos + correction[((int)dir[num] + 5) % 6];
                dp = direction[((int)dir[num] + 5) % 6];
            }
            else if (overdir == 1)
            {
                bp = lastPos + correction[(int)dir[num]];
                dp = direction[(int)dir[num]];
            }
            else if (overdir == 2)
            {
                bp = lastPos + correction[((int)dir[num] + 1) % 6];
                dp = direction[((int)dir[num] + 1) % 6];
            }
            else if (overdir == 3)
            {
                bp = lastPos + correction[((int)dir[num] + 2) % 6];
                dp = direction[((int)dir[num] + 2) % 6];
            }
            else if (overdir == 4)
            {
                bp = lastPos + correction[((int)dir[num] + 3) % 6];
                dp = direction[((int)dir[num] + 3) % 6];
            }
            else
            {
                bp = lastPos + correction[((int)dir[num] + 4) % 6];
                dp = direction[((int)dir[num] + 4) % 6];
            }

            // ��Ÿ�
            d = sb.distance[num];

            Debug.DrawRay(bp, dp * d, Color.blue, 2f);

            //hits = Physics2D.RaycastAll(bp, dp, d, LayerMask);
            //for (int i = 0; i < hits.Length; i++)
            //{
            //    RaycastHit2D hit = hits[i];
            //    hit.transform.GetComponent<SpriteRenderer>().color = Color.red;
            //    Debug.Log(hit.transform.name);
            //}

            int l;

            if (!team)
            {
                layerMask = 1 << 6 | 1 << 8;
                l = 6;

            }
            else
            {
                layerMask = 1 << 7 | 1 << 8;
                l = 7;
            }


            hits = Physics2D.RaycastAll(bp, dp, d, layerMask);
            Debug.Log("hits : " + hits.Length);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];

                if (hit.transform.gameObject.layer == 8)
                {
                    Debug.Log("this is E.T.");
                    Vector3Int v = map.WorldToCell(hit.transform.position);
                    List<GameObject> list = tilemapcontrol.getCharList(v);
                    if (list[0].transform.gameObject.layer == l)
                    {
                        if (sb.isDamage)
                        {
                            DataController.Instance.ModifyCharacterHp(
                                list[0].transform.gameObject.GetComponent<Character>().Pc.Id,
                                -sb.value);

                            GameObject o = PhotonNetwork.Instantiate(
                SpawnableSkillResources.GetPath(sb.sid),
                new Vector3(hit.transform.position.x, hit.transform.position.y + 0.1f, 0),
                Quaternion.identity);

                            if (!NullCheck.HasItComponent<SkillSpawner>(o, "SkillSpawner"))
                            {
                                // error
                                return;
                            }
                            //hit.transform.GetComponent<Character>().DamageHP(sb.value);
                        }
                    }
                }
                else if (hit.transform.gameObject.layer == l)
                {
                    if (sb.isDamage)
                    {
                        DataController.Instance.ModifyCharacterHp(
                            hit.transform.GetComponent<Character>().Pc.Id, -sb.value);
                        Debug.Log("hit!");

                        GameObject o = PhotonNetwork.Instantiate(
                    SpawnableSkillResources.GetPath(sb.sid),
                    new Vector3(hit.transform.position.x, hit.transform.position.y + 0.1f, 0),
                    Quaternion.identity);

                        if (!NullCheck.HasItComponent<SkillSpawner>(o, "SkillSpawner"))
                        {
                            // error
                            return;
                        }
                        //hit.transform.GetComponent<Character>().DamageHP(sb.value);
                    }
                }
            }
            lastPos = lastPos + dp * d;

            
        }

        public void CurvedMultipleRay(Vector2 basePos, SkillBase sb, List<Direction> dir, int overdir, bool team, int rays)
        {
            //if (reversed)
            //{
            //    for (int i = 0; i < rays; i++)
            //    {
            //        CurvedRay(basePos, sb, dir, i, reversed, highlight);
            //    }

            //}
            //else
            //{
            //    for (int i = 0; i < rays; i++)
            //    {
            //        CurvedRay(basePos, sb, dir, i, reversed, highlight);
            //    }
            //}

            if(!team)
            {
                for (int i = 0; i < rays; i++)
                {
                    CurvedRay(basePos, sb, dir, i, overdir, team);
                }
            }
            else
            {
                for (int i = 0; i < rays; i++)
                {
                    CurvedRay(basePos, sb, dir, i, overdir, team);
                }
            }
        }

        private void Start()
        {
            // ����ġ ����
            // ���� �� �밢��, ����, ���� �Ʒ� �밢��, ������ �Ʒ� �밢��, ������, ������ �� �밢�� ����
            correction.Add(Vector2.zero);
            correction.Add(new Vector2(0, 0.1f * yCor));
            correction.Add(Vector2.zero);
            correction.Add(Vector2.zero);
            correction.Add(new Vector2(0, 0.1f * yCor));
            correction.Add(Vector2.zero);

            // ���⺤�� ����
            // ���� ���� ����
            direction.Add(new Vector2(-0.5f * xCor, 0.5f * yCor));
            direction.Add(new Vector2(-1f * xCor, 0));
            direction.Add(new Vector2(-0.5f * xCor, -0.5f * yCor));
            direction.Add(new Vector2(0.5f * xCor, -0.5f * yCor));
            direction.Add(new Vector2(1f * xCor, 0));
            direction.Add(new Vector2(0.5f * xCor, 0.5f * yCor));
        }
    }

}