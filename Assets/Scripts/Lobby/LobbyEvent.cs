using Photon.Pun;
using Photon.Realtime;

using ExitGames.Client.Photon;

using System;

using UnityEngine;
using UnityEngine.UI;

namespace KWY
{
    public class LobbyEvent : MonoBehaviourPun
    {
        #region Private Serializable Fields
        [Tooltip("The button to send ready to start the game to server")]
        [SerializeField] private Button ReadyBtn;

        #endregion

        #region Private Fields

        [Tooltip("다음에 게임이 시작되면 로드될 scene")]
        readonly private string nextLevel = "";

        [Tooltip("Unique user id that the server determined")]
        private string UserId;
        #endregion

        #region Public Methods

        /// <summary>
        /// Send to 'ready signal' to the Server; content: [ready?: bool]
        /// </summary>
        public void RaiseEventReady()
        {
            byte evCode = (byte)EvCode.LobbyReady;
            object[] content = new object[]
            {
                true
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            if (PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions))
            {
                UtilForDebug.LogRaiseEvent(evCode, content, raiseEventOptions, sendOptions);
            }
            else
            {
                UtilForDebug.LogErrorRaiseEvent(evCode);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Callback method when the server raises events
        /// </summary>
        /// <param name="eventData">Received data from the server</param>
        private void OnEvent(EventData eventData)
        {
            UtilForDebug.LogData(eventData);

            switch (eventData.Code)
            {
                case (byte)EvCode.ResLobbyReady:
                    OnEventLobbyReady(eventData);
                    break;
                default:
                    Debug.LogError("There is not matching event code: " + eventData.Code);
                    break;
            }

        }

        /// <summary>
        /// Method when the server responses at client's LobbyReady event; data: [userId: string, resOk: bool, startgame: bool]
        /// </summary>
        /// <param name="eventData">Received data from the server</param>
        private void OnEventLobbyReady(EventData eventData)
        {
            object[] data = (object[])eventData.CustomData;

            if (UserId == (string)data[0] && (bool)data[1])
            {
                // 임시로 ready 완료되면 버튼 blue로 변경
                ReadyBtn.GetComponent<Image>().color = Color.blue;
            }

            // check 'start game?' through data[2]
            if ((bool)data[2])
            {
                Debug.Log("Start Game");

                // load next level
                // PhotonNetwork.LoadLevel(nextLevel);
            }
        }

        #endregion


        #region MonoBehaviourPun Callbacks
        private void Awake()
        {
            // when the scene loaded, get userid from PhotonNetwork.
            // 해당 함수가 실행되기전에 포톤 서버에 연결이 되있어야함
            try
            {
                UserId = PhotonNetwork.AuthValues.UserId;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.LogError("Can not get UserId - Check the server connection");
            }
        }

        public void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        public void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        #endregion
    }

}

