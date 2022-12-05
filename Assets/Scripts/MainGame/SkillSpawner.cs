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

        if(sb.isDamage)
        {
            if(sb.value > 0)
            {
                Debug.Log("this is sb.value > 0: " + sb.value);
                // not my skill
                if (gameObject.CompareTag("Enemy"))
                {
                    // 자신의 캐릭터(마스터 캐릭터)가 피격 당했을때
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
            else
            {
                Debug.Log("this is sb.value < 0: " + sb.value);
                // 체력 회복
                if (gameObject.CompareTag("Enemy"))
                {
                    // 상대방
                    if (collision.gameObject.CompareTag("Enemy"))
                    {
                        Debug.Log($"dmage: {damage}");
                        DataController.Instance.ModifyCharacterHp(
                            collision.gameObject.GetComponent<Character>().Pc.Id, damage);
                    }
                }

                if (gameObject.CompareTag("Friendly"))
                {
                    // 자신
                    if (collision.gameObject.CompareTag("Friendly"))
                    {
                        Debug.Log($"dmage: {damage}");
                        DataController.Instance.ModifyCharacterHp(
                            collision.gameObject.GetComponent<Character>().Pc.Id, damage);
                    }
                }
            }
        }
        else
        {
            // 추후 버프 추가하면 사용
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
