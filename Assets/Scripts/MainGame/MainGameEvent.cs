using Photon.Pun;
using Photon.Realtime;

using ExitGames.Client.Photon;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace KWY
{
    public class MainGameEvent : MonoBehaviourPun
    {
        #region Private Serializable Fields

        [Tooltip("The button to send ready to start simulation to server")]
        [SerializeField] private Button readyBtn;


        [Tooltip("The panel to show 'You win'")]
        [SerializeField] private GameObject winPanel;

        [Tooltip("The panel to show 'You lose'")]
        [SerializeField] private GameObject losePanel;

        [Tooltip("Gameobject that contains TurnReadyBtn functions")]
        [SerializeField] private GameObject turnReady;

        [SerializeField]
        private GameManager gameManager;

        #endregion

        #region Private Fields

        [Tooltip("다음에 게임이 시작되면 로드될 scene")]
        readonly private string nextLevel = "";

        [Tooltip("Unique user id that the server determined")]
        private string UserId;

       

        #endregion

        #region Public Methods

        /// <summary>
        /// Send to 'ready signal to start simulation' to the Server; content: actionData: Dictionary(int, string)
        /// </summary>
        public void RaiseEventTurnReady(ActionData actionData)
        {
            byte evCode = (byte)EvCode.TurnReady;

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            if (PhotonNetwork.RaiseEvent(evCode, actionData.Data, raiseEventOptions, sendOptions))
            {
                UtilForDebug.LogRaiseEvent(evCode, actionData.Data, raiseEventOptions, sendOptions);
            }
            else
            {
                UtilForDebug.LogErrorRaiseEvent(evCode);
            }
        }

        /// <summary>
        /// Send to 'Simulation end signal' to the Server; content: simulEnd?: bool(always)
        /// </summary>
        public void RaiseEventSimulEnd()
        {
            byte evCode = (byte)EvCode.SimulEnd;

            // 게임이 끝나지 않았을 경우 = true, 끝났을 경우 false
            var content = true;

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

        /// <summary>
        /// Send to 'Game ends' signal to the Server; It must be called ONLY Master Client; content: winnerId: string
        /// </summary>
        public void RaiseEventGameEnd()
        {
            byte evCode = (byte)EvCode.GameEnd;

            var content = UserId; // temp 이긴 유저의 id 값을 content로
            // content에 이긴 유저의 id 값 넣기

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

        public void RaiseEventPlayerSkill(PSID psid)
        {
            byte evCode = (byte)EvCode.PlayerSkill;

            object[] content = new object[]
            {
                (int)psid
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
                case (byte)EvCode.ResTurnReady:
                    OnEventTurnReady(eventData);
                    break;
                case (byte)EvCode.ResSimulEnd:
                    OnEventSimulEnd(eventData);
                    break;
                case (byte)EvCode.ResGameEnd:
                    OnEventGameEnd(eventData);
                    break;
                default:
                    //Debug.LogError("There is not matching event code: " + eventData.Code);
                    break;
            }

        }

        /// <summary>
        /// Method when the server responses at client's TurnReady event; data: [userId: string, resOk: bool, startSimul: bool, data?: Dictionary(int, string)]
        /// </summary>
        /// <param name="eventData">Received data from the server</param>
        private void OnEventTurnReady(EventData eventData)
        {
            object[] data = (object[])eventData.CustomData;

            if (UserId == (string)data[0] && (bool) data[1])
            {
                // 서버로 부터 ready에 대한 ok 사인이 왔을 때 변경함
                turnReady.GetComponent<TurnReadyBtn>().SetReady((bool)data[1]);
            }

            // check ' start simulation' through data[2]
            if ((bool) data[2])
            {
                Debug.Log("Start Simulation");

                // simulation을 master client일 경우만 실행 -> master client에서 object 액션 -> Photon 동기화 -> 다른 client에서도 똑같이 실행
                // 아직 테스트 하지 못하였음!!!!!!!!
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("Received simul data!!!!");
                    gameManager.SetState(1, (Dictionary<int, object[]>)data[3]); // Note: if data[2] is false, there is no data[3]
                }
                else
                {
                    Debug.Log("It is not matser client");
                    gameManager.SetState(1);
                }
            }
        }

        /// <summary>
        /// Method when the server responses the singal, 'simulation ended'; data: continue?: bool
        /// </summary>
        /// <param name="eventData">Received data from the server</param>
        private void OnEventSimulEnd(EventData eventData)
        {
            // 준비 상태 다시 초기화
            turnReady.GetComponent<TurnReadyBtn>().ResetReady();

            gameManager.SetState(0);
        }

        /// <summary>
        /// Method when the server responses the signal, 'the game ended'; data: [winnerId: string]
        /// </summary>
        /// <param name="eventData">Received data from the server</param>
        private void OnEventGameEnd(EventData eventData)
        {
            var data = eventData.CustomData;

            if (!(data is string))
            {
                Debug.LogError("Type Error");
                return;
            }

            // 이겻을 경우
            if ((string)data == this.UserId)
            {
                winPanel.SetActive(true);
            }
            // 졌을 경우
            else
            {
                losePanel.SetActive(true);
            }

            // 게임 종료 후 할 내용들 작성
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

        public void Start()
        {
            // ExitGames.Client.Photon.PhotonPeer.RegisterType(); //ㅠㅠ
        }

        #endregion
    }

}