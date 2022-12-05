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
        /// 게임 로비에서 자신 또는 상대방
        /// </summary>
        /// <param name="status">최신화 할 ready 상태</param>
        /// <param name="isMe">자신인지 아닌지</param>
        public void SetReadyStatus(bool status, bool isMe = true)
        {
            if (isMe)
            {
                myReady = status;
                LeftReadyText.SetActive(status);

                // ready 버튼 숨기기
                ReadyBtn.gameObject.SetActive(!status);

                // 취소 버튼 보이기
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

        // 플레이어에 custom property로 icon 가져올 수 있도록? -> 로그인 서버에서 처리?
        // 일단 null 값

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

            // 상대방 정보 없애기
            RightUserPanel.SetActive(false);

            // 상대 ready 취소 한 것처럼 보이기
            //SetReadyStatus(false, isMe: false);
            photonView.RPC("SetReadyStateRPC", RpcTarget.All, !PhotonNetwork.IsMasterClient, false);

            // 자신의 ready 상태 해제
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
            // 게임 시작
            if (PhotonNetwork.IsMasterClient)
            {
                //PhotonNetwork.LoadLevel(nextLevel);
                // game ready 서버로 전송
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
            // 상대 플레이어 입장 안했을 경우 레디 x
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

                // ready 버튼 숨기기
                ReadyBtn.gameObject.SetActive(!status);

                // 취소 버튼 보이기
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

            // join 한 경우 이미 들어와있는 플레이어 정보 로드
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
