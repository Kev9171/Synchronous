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

    private int damage = 0;

    private List<GameObject> clone = new List<GameObject>(); // Ŭ���� ����Ǵ� ��

    public void Activate(Vector2 basePos, Team casterTeam, int atk)
    {
        //clone.Add(Instantiate(target, new Vector3(basePos.x, basePos.y, 0), Quaternion.identity));

        GameObject o = PhotonNetwork.Instantiate(SpawnableSkillResources.GetPath(sb.sid),
            new Vector3(basePos.x, basePos.y, 0),
            Quaternion.identity);

        // note: is is called on only master client
        o.tag = casterTeam.Equals(Team.A) ? "Friendly" : "Enemy";

        damage = (int)(atk * sb.dmgMultiplicand);
    }

    public void Destroy(float time)
    {
        Debug.Log(gameObject.activeSelf);
        StartCoroutine(DestoryAfterTime(time));

        /*Destroy(clone[clone.Count - 1], time);
        clone.RemoveAt(clone.Count - 1);*/
    }

    IEnumerator DestoryAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(gameObject);
        clone.RemoveAt(clone.Count - 1);
        Debug.Log("Destoryed");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // not my skill
        if (gameObject.CompareTag("Enemy"))
        {
            // �ڽ��� ĳ����(������ ĳ����)�� �ǰ� ��������
            if (collision.gameObject.CompareTag("Friendly"))
            {
                DataController.Instance.ModifyCharacterHp(
                    collision.gameObject.GetComponent<Character>().Pc.Id, -damage);
            }
        }

        if (gameObject.CompareTag("Friendly"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
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
}
