using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KWY
{
    public class RayTest : MonoBehaviour
    {
        RaycastHit2D[] hits;
        public LayerMask LayerMask;
        private List<Vector2> correction = new List<Vector2>();
        private List<Vector2> direction = new List<Vector2>();

        public void Ray(Vector2 basePos, SkillBase sb)
        {

            //Debug.DrawRay(basePos + correction[0], direction[0] * sb.distance, Color.blue, 2f);
            Debug.DrawRay(basePos + correction[1], direction[1] * sb.distance, Color.blue, 2f);

            //hits = Physics2D.RaycastAll(basePos + correction[0], direction[0], sb.distance, LayerMask);
            hits = Physics2D.RaycastAll(basePos + correction[1], direction[1], sb.distance, LayerMask);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                hit.transform.GetComponent<SpriteRenderer>().color = Color.red;
                Debug.Log(hit.transform.name);
            }
        }

        private void Start()
        {
            // ����ġ ����
            //����, ���� �� �밢��, ������ �� �밢��, ������, ������ �Ʒ� �밢��, ���� �Ʒ� �밢�� ����
            correction.Add(new Vector2(0, 0.15f));
            correction.Add(Vector2.zero);
            correction.Add(Vector2.zero);
            correction.Add(new Vector2(0, 0.15f));
            correction.Add(Vector2.zero);
            correction.Add(Vector2.zero);

            // ���⺤�� ����
            // ���� ���� ����
            direction.Add(Vector2.left);
            direction.Add(new Vector2(-1f, 1f));
            direction.Add(new Vector2(1f, 1f));
            direction.Add(Vector2.right);
            direction.Add(new Vector2(1f, -1f));
            direction.Add(new Vector2(-1f, -1f));
        }
    }

}