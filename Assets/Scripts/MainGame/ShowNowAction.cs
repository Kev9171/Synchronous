using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace KWY
{
    /// <summary>
    /// Character와 Action, side를 인자로 받고 해당되는 사이드에 액션 보여주기
    /// </summary>
    public class ShowNowAction : MonoBehaviour
    {
        [SerializeField]
        private GameObject leftSkillLinePanel;
        [SerializeField]
        private GameObject rightSkillLinePanel;
        [SerializeField]
        private GameObject leftActionPiecePrefab;
        [SerializeField]
        private GameObject rightActionPiecePrefab;

        [SerializeField]
        MainGameData data;


        /// <summary>
        /// Called only by Master-Client
        /// </summary>
        /// <param name="id"></param>
        public void ShowMoveLog(int id)
        {
            PhotonView.Get(this).RPC("ShowMoveLogRPC", RpcTarget.Others, id);

            Sprite icon = data.PCharacters[id].Chara.Cb.icon;
            string name = "Move";
            float duration = 1f;

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(ShowActionLog(icon, name, data.IsMyCharacter[id], duration));
            }
            else
            {
                Debug.LogError("It can not be called on client");
            }
        }

        public void ShowSkillLog(int id, SID sid)
        {
            PhotonView.Get(this).RPC("ShowSkillLogRPC", RpcTarget.Others, id, sid);

            Sprite icon = data.PCharacters[id].Chara.Cb.icon;
            string name = SkillManager.GetData(sid).name;
            float duration = SkillManager.GetData(sid).castingTime;

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(ShowActionLog(icon, name, data.IsMyCharacter[id], duration));
            }
            else
            {
                Debug.LogError("It can not be called on client");
            }
        }

        [PunRPC]
        public void ShowMoveLogRPC(int id)
        {
            Sprite icon = data.PCharacters[id].Chara.Cb.icon;
            string name = "Move";
            float duration = 1f;

            if (!PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(ShowActionLog(icon, name, !data.IsMyCharacter[id], duration));
            }
            else
            {
                Debug.LogError("It can not be called on master-client");
            }
        }

        [PunRPC]
        public void ShowSkillLogRPC(int id, SID sid)
        {
            Sprite icon = data.PCharacters[id].Chara.Cb.icon;
            string name = SkillManager.GetData(sid).name;
            float duration = SkillManager.GetData(sid).castingTime;

            if (!PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(ShowActionLog(icon, name, !data.IsMyCharacter[id], duration));
            }
            else
            {
                Debug.LogError("It can not be called on master-client");
            }
        }

        IEnumerator ShowActionLog(Sprite icon, string name, bool left, float duration)
        {
            if (left)
            {
                Instantiate(leftActionPiecePrefab, leftSkillLinePanel.transform)
                    .GetComponent<ActionPiece>()
                    .SetDataAndStart(icon, name, duration);
            }
            else
            {
                Instantiate(rightActionPiecePrefab, rightSkillLinePanel.transform)
                    .GetComponent<ActionPiece>()
                    .SetDataAndStart(icon, name, duration);
            }

            yield return null;
        }
    }
}
