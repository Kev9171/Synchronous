#define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;

namespace KWY
{
    public class DataController : MonoBehaviour
    {
        public static DataController Instance;

        MainGameData data;
        PhotonView photonView;

        #region Data Control

        public void AddPlayerMp(int amount)
        {
            photonView.RPC("AddPlayerMpRPC", RpcTarget.All, amount);
        }

        [PunRPC]
        private void AddPlayerMpRPC(int amount)
        {
            MainGameData.Instance.MyPlayer.AddMp(amount);
        }

        public void AddAllCharactersMp(int amount)
        {
            photonView.RPC("AddAllCharactersMpRPC", RpcTarget.All, amount);
        }

        [PunRPC]
        private void AddAllCharactersMpRPC(int amount)
        {
            // 각각 클라이언트에서 실행
            foreach (PlayableCharacter pc in data.PCharacters.Values)
            {
                pc.Chara.AddMP(amount);
            }
        }

        public void ModifyCharacterHp(int id, int amount)
        {
            photonView.RPC("ModifyCharacterHpRPC", RpcTarget.All, id, amount);
        }

        [PunRPC]
        private void ModifyCharacterHpRPC(int id, int amount)
        {
            if (data.PCharacters.TryGetValue(id, out PlayableCharacter pc))
            {
                if (amount >= 0)
                {
                    pc.Chara.AddHp(amount);
                }
                else
                {
                    GameObject damageTextInstance = PhotonNetwork.Instantiate("TextHolder", pc.Chara.transform.position, Quaternion.identity, 0);
                    damageTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(amount.ToString());
                    pc.Chara.DamageHP(-amount);
                }
            }
#if TEST
            else
            {
                Debug.LogError($"Can not find character id={id}");
            }
#endif
        }

        public void ModifyCharacterMp(int id, int amount)
        {
            photonView.RPC("ModifyCharacterMpRPC", RpcTarget.All, id, amount);
        }

        [PunRPC]
        private void ModifyCharacterMpRPC(int id, int amount)
        {
            if (data.PCharacters.TryGetValue(id, out PlayableCharacter pc))
            {
                pc.Chara.AddMP(amount);
            }
#if TEST
            else
            {
                Debug.LogError($"Can not find character id={id}");
            }
#endif
        }

        #endregion

        private void Start()
        {
            photonView = PhotonView.Get(this);

            if (!photonView)
            {
                Debug.LogError($"Can not find photonview on {gameObject.name}");
                return;
            }

            data = MainGameData.Instance;

            Instance = this;
        }
        
    }
}