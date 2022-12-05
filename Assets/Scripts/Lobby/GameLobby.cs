using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

using Photon.Pun;

using PhotonPlayer = Photon.Realtime.Player;

using KWY;
using UI;

namespace Lobby
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

        PhotonView photonView;

        readonly private string nextLevel = "PickScene";
        //readonly private string nextLevel = "MainGameScene";
        readonly private string previousLevel = "StartScene";

        internal string myId;
        internal string otherId;

        float timeLimit;

        SLobbyData lobbyData;

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

        public void SetEnteredPlayer(PhotonPlayer player)
        {
            Debug.Log($"{PhotonNetwork.AuthValues.UserId}");

            RightUserPanel.GetComponent<UserProfilePanel>().SetData(null, player.NickName);
            RightUserPanel.SetActive(true);
        }

        public void ClearEnteredPlayer()
        {
            StopTimer();

            otherId = null;
            Debug.Log(otherId);

            // ���� ���� ���ֱ�
            RightUserPanel.SetActive(false);

            // ��� ready ��� �� ��ó�� ���̱�
            //SetReadyStatus(false, isMe: false);
            photonView.RPC("SetReadyStateRPC", RpcTarget.All, !PhotonNetwork.IsMasterClient, false);

            // �ڽ��� ready ���� ����
            //lobbyEvent.RaiseEventReady(false);
            photonView.RPC("SetReadyStateRPC", RpcTarget.All, PhotonNetwork.IsMasterClient, false);
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
            // Debug.Log(PhotonNetwork.AutomaticallySyncScene); // true
            // ���� ����
            if (PhotonNetwork.IsMasterClient)
            {
                //PhotonNetwork.LoadLevel(nextLevel);
                // game ready ������ ����
                lobbyEvent.RaiseEventGameReady();
            }

            Debug.Log("Game Start!");
        }

        public void LoadNextLevel()
        {
            PhotonNetwork.LoadLevel(nextLevel);
        }

        #region Button OnClick Callbacks

        public void OnReadyBtnClicked()
        {
            // ��� �÷��̾� ���� ������ ��� ���� x
            if (PhotonNetwork.CurrentRoom.PlayerCount != 2)
            {
                PanelBuilder.ShowFadeOutText(CanvasTransform, "There is no other player yet.");
                return;
            }

            photonView.RPC("SetReadyStateRPC", RpcTarget.All, PhotonNetwork.IsMasterClient, true);

            //lobbyEvent.RaiseEventReady(true);
        }

        [PunRPC]
        private void SetReadyStateRPC(bool isMaster, bool status)
        {
            if ((isMaster && PhotonNetwork.IsMasterClient) || (!isMaster && !PhotonNetwork.IsMasterClient))
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

            if (myReady && otherReady)
            {
                StartTimer();
            }
            else
            {
                StopTimer();
            }
        }

        [PunRPC]
        private void GetOtherPlayerIdRPC(string id)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            otherId = id;
            Debug.Log(otherId);
        }

        public void OnReadyCancelBtnClicked()
        {
            //lobbyEvent.RaiseEventReady(false);
            photonView.RPC("SetReadyStateRPC", RpcTarget.All, PhotonNetwork.IsMasterClient, false);
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
            photonView = PhotonView.Get(this);

            lobbyData = Resources.Load<SLobbyData>("Lobby/OLobbyData");

            if (!lobbyData)
            {
                Debug.LogError("Can not find lobbyData");
            }

            LeftUserPanel.GetComponent<UserProfilePanel>().LoadNowUser();

            // join �� ��� �̹� �����ִ� �÷��̾� ���� �ε�
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                foreach (PhotonPlayer p in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    if (p.UserId != PhotonNetwork.AuthValues.UserId)
                    {
                        SetEnteredPlayer(p);
                    }
                }
            }

            //this.timeLimit = MasterManager.GameSettings.GameLobbyTimerTime;
            timeLimit = lobbyData.startCountDownSeconds;

            TimerObject.InitTimer(timeLimit, TimeOut, CountDownText);

            LeaveRoomBtn.gameObject.AddComponent<GameLobbyBtnEventTrigeer>();
            ReadyBtn.gameObject.AddComponent<GameLobbyBtnEventTrigeer>();

            myId = PhotonNetwork.AuthValues.UserId;

            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("GetOtherPlayerIdRPC", RpcTarget.MasterClient, PhotonNetwork.AuthValues.UserId);
            }
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
