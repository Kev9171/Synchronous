using Photon.Pun;
using Photon.Realtime;

using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

using TMPro;

namespace KWY
{
    public class ConnectPhoton : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        GameObject StatusMsgPanel;

        [SerializeField]
        TMP_Text StatusMsg;

        [SerializeField]
        MasterManager _masterManager;

        const string connectingToServerMsg = "Connecting to Photon Server...";
        const string creatingRoomMsg = "Joining the room...";
        const string joingRoomMsg = "Creating the room...";

        const string lobbySceneName = "GameLobby";

        const string connectErrorMsg = "Failed to connect to server...\nCheck the internet connection and try again.";
        const string failedToCreateRoomMsg = "Failed to create the room, try again.";
        const string joiningRoomFaildMsg = "Failed to join the room, check the room name again.";

        public void ConnectPhotonServer()
        {
            StatusMsg.text = connectingToServerMsg;
            StatusMsgPanel.SetActive(true);

            PhotonNetwork.AutomaticallySyncScene = true; // 사용자 간 가장 쉬운 loading scene 방법
            PhotonNetwork.NickName = UserManager.UserName;
            PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

        public void CreateRoom(string roomName)
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("It is not connected to server");
                return;
            }
            StatusMsg.text = creatingRoomMsg;
            StatusMsgPanel.SetActive(true);

            RoomOptions options = new RoomOptions()
            {
                MaxPlayers = (byte)ConstantValue.MAX_PLAYERS,
                CustomRoomPropertiesForLobby = new string[] { ConstantValue.ELO_PROP_KEY },
                CustomRoomProperties = new Hashtable { { ConstantValue.ELO_PROP_KEY, ConstantValue.ELO_PROP_VALUE } }
            };

            PhotonNetwork.JoinOrCreateRoom(roomName, options, new TypedLobby(ConstantValue.TYPPED_LOBBY_SQL_NAME, LobbyType.SqlLobby));
        }

        public void JoinNamedRoom(string roomName)
        {
            StatusMsg.text = joingRoomMsg;
            StatusMsgPanel.SetActive(true);
            // roomName should not be empty
            PhotonNetwork.JoinRoom(roomName);
        }


        private void QuitGame()
        {
            Application.Quit();
        }

        #region Override Callbacks about Connecting to Server
        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Photon.", this);
            Debug.Log("My Nickname is " + PhotonNetwork.LocalPlayer.NickName, this);
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Failed to connect to Photon: " + cause.ToString(), this);

            // 로그아웃 요청
            StartCoroutine(LoginJoinAPI.Instance.LogoutPost(UserManager.AccountId));

            // 연결 실패할 경우 오류 팝업 띄우기
            GameObject canvas = GameObject.Find("Canvas");
            PopupBuilder.ShowErrorPopup(canvas.transform, ErrorCode.PHOTON_DISCONNECT, QuitGame);
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined a lobby");
            StatusMsgPanel.SetActive(false);
        }

        #endregion

        #region Override Callbacks about Joining a Room

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined a Room");
            StatusMsgPanel.SetActive(false);

            PhotonNetwork.LoadLevel(lobbySceneName);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("Joining Room Failed: {0}, code: {1}", message, returnCode);

            StatusMsgPanel.SetActive(false);

            GameObject canvas = GameObject.Find("Canvas");
            PopupBuilder.ShowPopup(canvas.transform, joiningRoomFaildMsg);
        }

        #endregion

        #region Override Callbacks about Room
        public override void OnCreatedRoom()
        {
            Debug.Log("Created room successfully.", this);

            StatusMsgPanel.SetActive(false);

            // createRoom 이후에 바로 joinroom이 되기 때문에 씬 변경은
            // Joinroom callback 에서 처리
            // 여기서 중복으로 PhotonNetwork.LoadLevel 하면 중복으로 씬 전환 안됨!!!!
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError("Room creation failed: " + message, this);

            StatusMsgPanel.SetActive(false);

            GameObject canvas = GameObject.Find("Canvas");
            PopupBuilder.ShowPopup(canvas.transform, failedToCreateRoomMsg);
        }

        #endregion

    }
}
