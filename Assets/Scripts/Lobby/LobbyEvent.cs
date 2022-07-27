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

            if (UserId == (string)data[0] && (bool)data[1])
            {
                // ready 상태 버튼 보여주기
                gameLobby.ShowReadyStatus(true);
            }
            // ready 유저가 자신이 아닐 경우 right setactive(true);
            else if (UserId != (string)data[0] && (bool)data[1])
            {
                gameLobby.ShowReadyStatus(false);
            }
            // ready 응답을 받았는데 이미 ready 일 경우 ready 취소
            else if (UserId == (string)data[0] && (bool)data[1] && gameLobby.myReady)
            {
                gameLobby.myReady = false;

                gameLobby.HideReadyStatus(true);
                gameLobby.StopTimer();
                gameLobby.ResetTimer();
            }
            else if (UserId != (string)data[0] && (bool)data[1] && gameLobby.otherReady)
            {
                gameLobby.otherReady = false;

                gameLobby.HideReadyStatus(false);
                gameLobby.StopTimer();
                gameLobby.ResetTimer();
            }

            // check 'start game?' through data[2]
            if ((bool)data[2])
            {
                gameLobby.StartTimer();

                Debug.Log("Start Game");

                // load next level
                //PhotonNetwork.LoadLevel(nextLevel);
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

