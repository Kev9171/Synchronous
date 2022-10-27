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

    public void Init(Team casterTeam, int atk)
    {
        gameObject.tag = casterTeam.Equals(Team.A) ? "Friendly" : "Enemy";
        damage = (int)(atk * sb.dmgMultiplicand);
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
    }

    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DestoryAfterTime(sb.triggerTime));
        }
    }
}
