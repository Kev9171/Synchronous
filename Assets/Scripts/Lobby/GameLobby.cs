using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Collections;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

namespace KWY
{
    public class GameLobby : MonoBehaviour
    {
        [SerializeField]
        GameObject LeftUserPanel;

        [SerializeField]
        GameObject LeftReadyText;

        [SerializeField]
        GameObject RightUserPanel;

        [SerializeField]
        GameObject RightReadyText;

        [SerializeField]
        Button LeaveRoomBtn;

        [SerializeField]
        TMP_Text CountDownText;

        [SerializeField]
        Button ReadyBtn;

        [SerializeField]
        Button ReadyCancelBtn;

        [SerializeField]
        Button HelpBtn;

        [SerializeField]
        Button SettingBtn;

        [SerializeField]
        LobbyEvent lobbyEvent;

        [SerializeField]
        Transform CanvasTransform;

        [Tooltip("������ ������ ���۵Ǹ� �ε�� scene")]
        readonly private string nextLevel = "MainGameScene";

        readonly private string previousLevel = "StartScene";

        private float time;
        float timeLimit;

        public bool myReady { get; set; } = false;
        public bool otherReady { get; set; } = false;

        const string leaveRoomMsg = "Do you want to leave this room?";

        public void ShowReadyStatus(bool isMe)
        {
            if (isMe)
            {
                myReady = true;
                LeftReadyText.SetActive(true);
            }
            else
            {
                otherReady = true;
                RightReadyText.SetActive(true);
            }
        }

        public void HideReadyStatus(bool isMe)
        {
            if (isMe)
            {
                myReady = false;
                LeftReadyText.SetActive(false);
            }
            else
            {
                otherReady = false;
                RightReadyText.SetActive(false);
            }
        }

        // �÷��̾ custom property�� icon ������ �� �ֵ���? -> �α��� �������� ó��?
        // �ϴ� null ��

        public void SetEnteredPlayer(Player player)
        {
            RightUserPanel.GetComponent<UserProfilePanel>().SetData(null, player.NickName);
            RightUserPanel.SetActive(true);
        }

        public void ClearEnteredPlayer()
        {
            RightUserPanel.SetActive(false);
        }

        #region CountDown

        public void StartTimer()
        {
            time = 0;
            StartCoroutine(Timer());
        }

        public void ResetTimer()
        {
            time = 0;
        }

        public void StopTimer()
        {
            StopCoroutine(Timer());
        }

        private void TimeOut()
        {
            // ���� ����
            PhotonNetwork.LoadLevel(nextLevel);
        }

        IEnumerator Timer()
        {
            while (true)
            {
                float t = Mathf.Ceil(timeLimit - time);

                if (t < 0)
                {
                    CountDownText.text = "0";
                    TimeOut();
                    break;
                }
                else
                {
                    CountDownText.text = t.ToString();
                    time += 0.5f;
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        #endregion

        #region Button OnClick Callbacks

        public void OnReadyBtnClicked()
        {
            ReadyBtn.gameObject.SetActive(false);
            ReadyCancelBtn.gameObject.SetActive(true);
            lobbyEvent.RaiseEventReady(true);
        }

        public void OnReadyCancelBtnClicked()
        {
            ReadyCancelBtn.gameObject.SetActive(false);
            ReadyBtn.gameObject.SetActive(true);
            lobbyEvent.RaiseEventReady(false);
        }

        public void OnLeaveRoomBtnClicked()
        {
            PopupBuilder.ShowPopup(CanvasTransform, 
                leaveRoomMsg,
                OnLeaveRoomBtnClickedCallback, true);
        }

        public void OnSettingBtnClicked()
        {
            PopupBuilder.ShowSettingPanel(CanvasTransform, null);
        }

        public void OnHelpBtnClicked()
        {

        }

        public void OnLeaveRoomBtnClickedCallback()
        {
            PhotonNetwork.LeaveRoom(false);

            SceneManager.LoadScene(previousLevel);
        }

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            // logic data���� �ҷ�����
            timeLimit = 10f;

            LeftUserPanel.GetComponent<UserProfilePanel>().LoadNowUser();

            // join �� ��� �̹� �����ִ� �÷��̾� ���� �ε�
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    if (p.UserId != PhotonNetwork.AuthValues.UserId)
                    {
                        SetEnteredPlayer(p);
                    }
                }
            }
        }

        #endregion
    }
}
