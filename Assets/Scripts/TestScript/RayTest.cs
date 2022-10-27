using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;

namespace KWY
{
    public class RayTest : MonoBehaviour
    {
        [SerializeField]
        TilemapControl tilemapcontrol;
        [SerializeField]
        Tilemap map;

        public LayerMask layerMask;
        RaycastHit2D[] hits;
        private List<Vector2> correction = new List<Vector2>();
        private List<Vector2> direction = new List<Vector2>();
        private Vector2 lastPos;
        private float xCor = 0.65f * 1.5f;  //맵1 -> 맵2 x값 보정치
        private float yCor = 0.7f * 1.5f / 0.65f;  // 맵1 -> 맵2 y값 보정치

        public void Ray(Vector2 basePos, SkillBase sb, int dir, bool reversed)
        {
            Vector2 bp; // 위치 보정된 원점
            Vector2 dp; // 방향 벡터
            float d;    // 사거리

            if (reversed)
            {
                // 반전일 경우
                bp = basePos + correction[5 - dir];
                dp = direction[5 - dir];
            }
            else
            {
                // 아닐 경우
                bp = basePos + correction[dir];
                dp = direction[dir];
            }

            //// 사거리
            //if (dir == 1 || dir == 4)
            //{
            //    d = sb.distance[dir];
            //}
            //else
            //{
            //    d = (sb.distance[dir]) / math.sqrt(2);
            //    d = sb.distance[dir];
            //}

            // 사거리
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

        public void CurvedRay(Vector2 basePos, SkillBase sb, List<Direction> dir, int num, bool reversed)
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

            // 반전일 경우
            if (reversed)
            {
                bp = lastPos + correction[5 - (int)dir[num]];
                dp = direction[5 - (int)dir[num]];
            }
            // 아닐 경우
            else
            {
                bp = lastPos + correction[(int)dir[num]];
                dp = direction[(int)dir[num]];
            }

            // 사거리
            d = sb.distance[num];

            Debug.DrawRay(bp, dp * d, Color.blue, 2f);

            //hits = Physics2D.RaycastAll(bp, dp, d, LayerMask);
            //for (int i = 0; i < hits.Length; i++)
            //{
            //    RaycastHit2D hit = hits[i];
            //    hit.transform.GetComponent<SpriteRenderer>().color = Color.red;
            //    Debug.Log(hit.transform.name);
            //}


            hits = Physics2D.RaycastAll(bp, dp, d, layerMask);
            Debug.Log("hits : " + hits.Length);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                hit.transform.GetComponent<SpriteRenderer>().color = Color.red;

                if (hit.transform.gameObject.layer == 6)
                {
                    if (sb.isDamage)
                    {
                        DataController.Instance.ModifyCharacterHp(
                            hit.transform.GetComponent<Character>().Pc.Id, -sb.value);
                        //hit.transform.GetComponent<Character>().DamageHP(sb.value);
                    }
                }
                else if (hit.transform.gameObject.layer == 8)
                {
                    Vector3Int v = map.WorldToCell(hit.transform.position);
                    List<GameObject> list = tilemapcontrol.getCharList(v);
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].transform.gameObject.layer == 6)
                        {
                            if (sb.isDamage)
                            {
                                DataController.Instance.ModifyCharacterHp(
                                    hit.transform.GetComponent<Character>().Pc.Id,
                                    -sb.value);
                                //hit.transform.GetComponent<Character>().DamageHP(sb.value);
                            }
                        }
                    }
                }
            }
            lastPos = lastPos + dp * d;
        }

        public void CurvedMultipleRay(Vector2 basePos, SkillBase sb, List<Direction> dir, bool reversed, int rays)
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

            for (int i = 0; i < rays; i++)
            {
                CurvedRay(basePos, sb, dir, i, reversed);
            }
        }

        private void Start()
        {
            // 보정치 저장
            // 왼쪽 위 대각선, 왼쪽, 왼쪽 아래 대각선, 오른쪽 아래 대각선, 오른쪽, 오른쪽 위 대각선 순서
            correction.Add(Vector2.zero);
            correction.Add(new Vector2(0, 0.1f * yCor));
            correction.Add(Vector2.zero);
            correction.Add(Vector2.zero);
            correction.Add(new Vector2(0, 0.1f * yCor));
            correction.Add(Vector2.zero);

            // 방향벡터 저장
            // 위와 순서 동일
            direction.Add(new Vector2(-0.5f * xCor, 0.5f * yCor));
            direction.Add(new Vector2(-1f * xCor, 0));
            direction.Add(new Vector2(-0.5f * xCor, -0.5f * yCor));
            direction.Add(new Vector2(0.5f * xCor, -0.5f * yCor));
            direction.Add(new Vector2(1f * xCor, 0));
            direction.Add(new Vector2(0.5f * xCor, 0.5f * yCor));
        }
    }

}