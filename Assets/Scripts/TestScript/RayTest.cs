using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;

namespace KWY
{
    public class RayTest : MonoBehaviour
    {
        RaycastHit2D[] hits;
        public LayerMask LayerMask;
        private List<Vector2> correction = new List<Vector2>();
        private List<Vector2> direction = new List<Vector2>();

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

            // ��Ÿ�
            if (dir == 1 || dir == 4)
            {
                d = sb.distance[dir];
            }
            else
            {
                d = (sb.distance[dir]) / math.sqrt(2);
            }

            Debug.DrawRay(bp, dp * d, Color.blue, 2f);
            hits = Physics2D.RaycastAll(bp, dp, d, LayerMask);

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                hit.transform.GetComponent<SpriteRenderer>().color = Color.red;
                Debug.Log(hit.transform.name);
            }
        }

        public void MultipleRay(Vector2 basePos, SkillBase sb, List<Direction> dir, bool reversed, int rays)
        {
            if(reversed)
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

        private void Start()
        {
            // ����ġ ����
            // ���� �� �밢��, ����, ���� �Ʒ� �밢��, ������ �Ʒ� �밢��, ������, ������ �� �밢�� ����
            correction.Add(Vector2.zero);
            correction.Add(new Vector2(0, 0.15f));
            correction.Add(Vector2.zero);
            correction.Add(Vector2.zero);
            correction.Add(new Vector2(0, 0.15f));
            correction.Add(Vector2.zero);

            // ���⺤�� ����
            // ���� ���� ����
            direction.Add(new Vector2(-1f, 1f));
            direction.Add(Vector2.left);
            direction.Add(new Vector2(-1f, -1f));
            direction.Add(new Vector2(1f, -1f));
            direction.Add(Vector2.right);
            direction.Add(new Vector2(1f, 1f));
        }
    }

}