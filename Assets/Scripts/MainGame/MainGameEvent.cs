using Photon.Pun;
using Photon.Realtime;

using ExitGames.Client.Photon;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using DebugUtil;

namespace KWY
{
    public class MainGameEvent : MonoBehaviourPun
    {

        [Tooltip("The button to send ready to start simulation to server")]
        [SerializeField] private Button readyBtn;

        [Tooltip("다음에 게임이 시작되면 로드될 scene")]
        readonly private string nextLevel = "";

        [Tooltip("Unique user id that the server determined")]
        private string UserId;

        private GameObject UICanvas;

        public static MainGameEvent Instance;
       


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
                DebugLog.LogRaiseEvent(evCode, actionData.Data, raiseEventOptions, sendOptions);
            }
            else
            {
                DebugLog.FailedToRaiseEvent(evCode);
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
                DebugLog.LogRaiseEvent(evCode, content, raiseEventOptions, sendOptions);
            }
            else
            {
                DebugLog.FailedToRaiseEvent(evCode);
            }
        }

        /// <summary>
        /// Send to 'Game ends' signal to the Server; It must be called ONLY Master Client; content: winnerTeam: int
        /// </summary>
        public void RaiseEventGameEnd(TICK_RESULT result)
        {
            byte evCode = (byte)EvCode.GameEnd;

            var content = result switch
            {
                TICK_RESULT.DRAW => ((int)TICK_RESULT.DRAW),
                TICK_RESULT.MASTER_WIN => ((int)TICK_RESULT.MASTER_WIN),
                TICK_RESULT.CLIENT_WIN => ((int)TICK_RESULT.CLIENT_WIN),
                _ => -1,
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
                DebugLog.LogRaiseEvent(evCode, content, raiseEventOptions, sendOptions);
            }
            else
            {
                DebugLog.FailedToRaiseEvent(evCode);
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
                DebugLog.LogRaiseEvent(evCode, content, raiseEventOptions, sendOptions);
            }
            else
            {
                DebugLog.FailedToRaiseEvent(evCode);
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
                readyBtn.GetComponent<TurnReadyBtn>().SetReady((bool)data[1]);
            }

            // check ' start simulation' through data[2]
            if ((bool) data[2])
            {
                Debug.Log("Start Simulation");

                // simulation을 master client일 경우만 실행 -> master client에서 object 액션 -> Photon 동기화 -> 다른 client에서도 똑같이 실행
                // 아직 테스트 하지 못하였음!!!!!!!!
                if (PhotonNetwork.IsMasterClient)
                {
                    GameManager.Instance.SetState(STATE.Simul, (Dictionary<int, object[]>)data[3]); // Note: if data[2] is false, there is no data[3]
                }
                else
                {
                    GameManager.Instance.SetState(STATE.Simul);
                }
            }
        }

        /// <summary>
        /// Method when the server responses the singal, 'simulation ended'; data: continue?: bool
        /// </summary>
        /// <param name="eventData">Received data from the server</param>
        private void OnEventSimulEnd(EventData eventData)
        {
            GameManager.Instance.SetState(STATE.TurnReady);
        }

        /// <summary>
        /// Method when the server responses the signal, 'the game ended'; data: [TICK_RESULT: int]
        /// </summary>
        /// <param name="eventData">Received data from the server</param>
        private void OnEventGameEnd(EventData eventData)
        {
            var data = eventData.CustomData;

            TICK_RESULT result = (TICK_RESULT)data;

            GameManager.Instance.SetState(STATE.GameOver, result);
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
                Debug.LogError("Can not get UserId - Check the server connection");
            }

            UICanvas = GameObject.Find("UICanvas");

            if (!UICanvas)
            {
                Debug.Log("Can not find gameobject named: UICanvas");
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
            Instance = this;
            // ExitGames.Client.Photon.PhotonPeer.RegisterType(); //ㅠㅠ
        }

        #endregion
    }

}