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



        public void ShowMoveLog(int charaCid)
        {
            PhotonView.Get(this).RPC("ShowMoveLogRPC", RpcTarget.Others, charaCid);

            int t = charaCid;
            if (charaCid > 100)
            {
                charaCid -= 100;
            }

            Sprite icon = CharaManager.GetData((CID)charaCid).icon;
            string name = "Move";
            StartCoroutine(ShowActionLog(icon, name, t < 100));

        }

        public void ShowSkillLog(int charaCid, SID sid)
        {
            PhotonView.Get(this).RPC("ShowSkillLogRPC", RpcTarget.Others, charaCid, sid);

            int t = charaCid;
            if (charaCid > 100)
            {
                charaCid -= 100;
            }

            Sprite icon = CharaManager.GetData((CID)charaCid).icon;
            string name = SkillManager.GetData(sid).name;
            StartCoroutine(ShowActionLog(icon, name, t < 100));
        }

        [PunRPC]
        public void ShowMoveLogRPC(int charaCid)
        {
            int t = charaCid;
            if (charaCid > 100)
            {
                charaCid -= 100;
            }

            Sprite icon = CharaManager.GetData((CID)charaCid).icon;
            string name = "Move";
            StartCoroutine(ShowActionLog(icon, name, t < 100));

        }

        [PunRPC]
        public void ShowSkillLogRPC(int charaCid, SID sid)
        {
            int t = charaCid;
            if (charaCid > 100)
            {
                charaCid -= 100;
            }

            Sprite icon = CharaManager.GetData((CID)charaCid).icon;
            string name = SkillManager.GetData(sid).name;
            StartCoroutine(ShowActionLog(icon, name, t < 100));
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
