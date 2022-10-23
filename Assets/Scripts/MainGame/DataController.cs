#define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

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
            // ���� Ŭ���̾�Ʈ���� ����
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