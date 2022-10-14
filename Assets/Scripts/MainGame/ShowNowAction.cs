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

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(ShowActionLog(icon, name, data.IsMyCharacter[id]));
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

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(ShowActionLog(icon, name, data.IsMyCharacter[id]));
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

            if (!PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(ShowActionLog(icon, name, !data.IsMyCharacter[id]));
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

            if (!PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(ShowActionLog(icon, name, !data.IsMyCharacter[id]));
            }
            else
            {
                Debug.LogError("It can not be called on master-client");
            }
        }

        IEnumerator ShowActionLog(Sprite icon, string name, bool left)
        {
            if (left)
            {
                Instantiate(leftActionPiecePrefab, leftSkillLinePanel.transform).GetComponent<ActionPiece>().SetData(icon, name);
                //GameObject o = PhotonNetwork.Instantiate("Prefabs/UI/Main/LeftSKill", Vector3.zero, Quaternion.identity);
                //o.transform.parent = leftSkillLinePanel.transform;
                //o.GetComponent<ActionPiece>().SetData(icon, name);
            }
            else
            {
                Instantiate(rightActionPiecePrefab, rightSkillLinePanel.transform).GetComponent<ActionPiece>().SetData(icon, name);
                //GameObject o = PhotonNetwork.Instantiate("Prefabs/UI/Main/RightSkill", Vector3.zero, Quaternion.identity);
                //o.transform.parent = rightSkillLinePanel.transform;
                //o.GetComponent<ActionPiece>().SetData(icon, name);
            }

            yield return null;
        }
    }
}
