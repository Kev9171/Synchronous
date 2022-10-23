using Photon.Pun;
using Photon.Realtime;

using ExitGames.Client.Photon;

using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace KWY
{
    public class LobbyEvent : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        GameLobby gameLobby;

        #region Private Fields

        [Tooltip("Unique user id that the server determined")]
        private string UserId;

        #endregion

        #region Public Methods

        /// <summary>
        /// Send to 'ready signal' to the Server; content: [ready?: bool]
        /// </summary>
        public void RaiseEventReady(bool isReady)
        {
            byte evCode = (byte)EvCode.LobbyReady;
            object[] content = new object[]
            {
                isReady
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
            switch (eventData.Code)
            {
                case (byte)EvCode.ResLobbyReady:
                    OnEventLobbyReady(eventData);
                    break;
                default:
                    //Debug.LogError("There is not matching event code: " + eventData.Code);
                    break;
            }

        }

        /// <summary>
        /// Method when the server responses at client's LobbyReady event; data: [userId: string, resOk: bool, startgame: bool]
        /// </summary>
        /// <param name="eventData">Received data from the server</param>
        private void OnEventLobbyReady(EventData eventData)
        {
            UserId = PhotonNetwork.AuthValues.UserId; // temp
            object[] data = (object[])eventData.CustomData;

            // 자신에 대한 이벤트 일 경우
            if (UserId == (string)data[0])
            {
                // ready 상태 최신화에 대한 ok 사인을 받았으면
                if ((bool)data[1])
                {
                    
                    gameLobby.SetReadyStatus((bool)data[2]);
                }
                else
                {
                    // error
                }
            }
            // 상대방 id
            else
            {
                // ready 상태 최신화에 대한 ok 사인을 받았으면
                if ((bool)data[1])
                {
                    // 서버에서 최신화된 ready 상태 : data[2]
                    gameLobby.SetReadyStatus((bool)data[2], isMe: false);
                }
                else
                {
                    // error
                }
            }

            // check 'start game?' through data[2]
            if ((bool)data[3])
            {
                gameLobby.StartTimer();
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


        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("New player entered the room: " + newPlayer.NickName); ;

            gameLobby.SetEnteredPlayer(newPlayer);
            base.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("New player left the room: " + otherPlayer.NickName); ;

            gameLobby.ClearEnteredPlayer();
            base.OnPlayerLeftRoom(otherPlayer);
        }

        #endregion
    }

}

