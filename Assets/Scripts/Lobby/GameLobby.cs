using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

        [SerializeField]
        Timer TimerObject;

        readonly private string nextLevel = "PickScene"; // MainGameScene
        readonly private string previousLevel = "StartScene";

        float timeLimit;

        public bool myReady { get; set; } = false;
        public bool otherReady { get; set; } = false;

        const string leaveRoomMsg = "Do you want to leave this room?";

        /// <summary>
        /// ���� �κ񿡼� �ڽ� �Ǵ� ����
        /// </summary>
        /// <param name="status">�ֽ�ȭ �� ready ����</param>
        /// <param name="isMe">�ڽ����� �ƴ���</param>
        public void SetReadyStatus(bool status, bool isMe = true)
        {
            if (isMe)
            {
                myReady = status;
                LeftReadyText.SetActive(status);

                // ready ��ư �����
                ReadyBtn.gameObject.SetActive(!status);

                // ��� ��ư ���̱�
                ReadyCancelBtn.gameObject.SetActive(status);
            }
            else
            {
                otherReady = status;
                RightReadyText.SetActive(status);
            }

            if (!status)
            {
                StopTimer();
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
            StopTimer();

            // ���� ���� ���ֱ�
            RightUserPanel.SetActive(false);

            // ��� ready ��� �� ��ó�� ���̱�
            SetReadyStatus(false, isMe: false);

            // �ڽ��� ready ���� ����
            lobbyEvent.RaiseEventReady(false);
        }

        public void StartTimer()
        {
            CountDownText.gameObject.SetActive(true);
            TimerObject.StartTimer();
        }

        public void StopTimer()
        {
            CountDownText.gameObject.SetActive(false);
            TimerObject.StopTimer();
        }

        private void TimeOut()
        {
            // ���� ����
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(nextLevel);
            }

            Debug.Log("Game Start!");
        }

        #region Button OnClick Callbacks

        public void OnReadyBtnClicked()
        {
            // ��� �÷��̾� ���� ������ ��� ���� x
            if (PhotonNetwork.CurrentRoom.PlayerCount != 2)
            {
                return;
            }

            lobbyEvent.RaiseEventReady(true);
        }

        public void OnReadyCancelBtnClicked()
        {            
            lobbyEvent.RaiseEventReady(false);
        }

        public void OnLeaveRoomBtnClicked()
        {
            PopupBuilder.ShowPopup2(CanvasTransform, 
                leaveRoomMsg,
                OnLeaveRoomBtnClickedCallback);
        }

        public void OnSettingBtnClicked()
        {
            PopupBuilder.ShowSettingPanel(CanvasTransform, null);
        }

        public void OnHelpBtnClicked()
        {
            PopupBuilder.ShowGameLobbyHelpPopup(CanvasTransform, null);
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

            this.timeLimit = MasterManager.GameSettings.GameLobbyTimerTime;

            TimerObject.InitTimer(timeLimit, TimeOut, CountDownText);

            LeaveRoomBtn.gameObject.AddComponent<GameLobbyBtnEventTrigeer>();
            ReadyBtn.gameObject.AddComponent<GameLobbyBtnEventTrigeer>();
        }

        private void OnEnable()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void OnDisable()
        {
            PhotonNetwork.AutomaticallySyncScene = false;
        }

        #endregion
    }

    public class GameLobbyBtnEventTrigeer : EventTrigger
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.05f, 1.05f, 1.05f);
            base.OnPointerEnter(eventData);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            base.OnPointerExit(eventData);
        }
    }
}
