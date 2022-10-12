using KWY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSpawner : MonoBehaviour
{
    public GameObject target;
    private List<GameObject> clone = new List<GameObject>(); // 클론이 저장되는 곳

    public void Activate(Vector2 basePos)
    {
        clone.Add(Instantiate(target, new Vector3(basePos.x, basePos.y, 0), Quaternion.identity));
    }

    public void Destroy(float time)
    {
        Destroy(clone[clone.Count - 1], time);
        clone.RemoveAt(clone.Count - 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Character>().DamageHP(5);
        }
    }
}
