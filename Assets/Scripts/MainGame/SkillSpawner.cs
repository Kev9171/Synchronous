using KWY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SkillSpawner : MonoBehaviour
{
    public GameObject target;

    [SerializeField]
    SkillBase sb;

    public int damage = 0;

    //private List<GameObject> clone = new List<GameObject>(); // Ŭ���� ����Ǵ� ��

    public void Init(Team casterTeam, int atk)
    {
        gameObject.tag = casterTeam.Equals(Team.A) ? "Friendly" : "Enemy";
        damage = (int)(atk * sb.dmgMultiplicand);
    }

    public void Activate(Vector2 basePos, Team casterTeam, int atk)
    {
        //clone.Add(Instantiate(target, new Vector3(basePos.x, basePos.y, 0), Quaternion.identity));

        GameObject o = PhotonNetwork.Instantiate(SpawnableSkillResources.GetPath(sb.sid),
            new Vector3(basePos.x, basePos.y + 0.1f, 0),
            Quaternion.identity);

        // note: is is called on only master client
        o.tag = casterTeam.Equals(Team.A) ? "Friendly" : "Enemy";
        //clone.Add(o);

        damage = (int)(atk * sb.dmgMultiplicand);
        Debug.Log($"dmage: {damage}");
    }

    IEnumerator DestoryAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(gameObject);
        //clone.RemoveAt(clone.Count - 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        Debug.Log($"caster:{gameObject.tag}, collider.gameObject: {collision.gameObject.tag}");

        // not my skill
        if (gameObject.CompareTag("Enemy"))
        {
            // �ڽ��� ĳ����(������ ĳ����)�� �ǰ� ��������
            if (collision.gameObject.CompareTag("Friendly"))
            {
                Debug.Log($"dmage: {damage}");
                DataController.Instance.ModifyCharacterHp(
                    collision.gameObject.GetComponent<Character>().Pc.Id, -damage);
            }
        }

        if (gameObject.CompareTag("Friendly"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log($"dmage: {damage}");
                DataController.Instance.ModifyCharacterHp(
                    collision.gameObject.GetComponent<Character>().Pc.Id, -damage);
            }
        }

        /*if (collision.gameObject.CompareTag("Enemy"))
        {
            //collision.gameObject.GetComponent<Character>().DamageHP(30);
            // -30 is temp value;
            DataController.Instance.ModifyCharacterHp(
                collision.gameObject.GetComponent<Character>().Pc.Id, -30);
        }*/
    }

    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DestoryAfterTime(sb.triggerTime));
        }
    }
}
