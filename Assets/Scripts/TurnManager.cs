using Photon.Pun;
using Photon.Realtime;

using ExitGames.Client.Photon;

using UnityEngine;

namespace KWY
{
    class TurnManager : MonoBehaviourPun
    {
        #region Constant Fields
        const byte StartReadyTurnCode = 0;
        const byte StartSimulCode = 1;
        const byte PlayerSkillTrigCode = 2;

        const byte ReadyCode = 10;
        const byte EndSimulCode = 11;

        #endregion


        #region Public Fields
        [Tooltip("True - now simulating; False - 턴 준비 단계")]
        public bool NowSimul;

        /// <summary>
        /// Every event is received through this function.
        /// 모든 이벤트는 이 함수를 통해 받을 수 있음
        /// </summary>
        /// <param name="eventData"> received data from the server</param>
        public void OnEvent(EventData eventData) 
        {
            // for debug
            Debug.Log("FunCalled - OnEvent");
            Debug.LogFormat("EventData: {0}", eventData);
            Debug.LogFormat("evCode: {0}", eventData.Code);

            byte evCode = eventData.Code; // event code

            // branch
            switch(evCode)
            {
                case StartReadyTurnCode:
                    StartReadyTurn();
                    break;
                case StartSimulCode:
                    TimeTable tt = (TimeTable) eventData.CustomData;
                    StartSimul(tt);
                    break;
                case PlayerSkillTrigCode:
                    TimeTable tt2 = (TimeTable) eventData.CustomData;
                    ModifySimul(tt2);
                    break;
                default:
                    Debug.LogFormat("There is no match code with {0}", evCode);
                    break;
            }

        }

        /// <summary>
        /// Send ready state to the Server
        /// </summary>
        public void RaiseEventSendReady()
        {
            Debug.Log("RaiseEventSendRead Called");
            byte evCode = ReadyCode;
            object[] content = new object[] {};

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            };

            SendOptions sendOptions = new SendOptions {
                Reliability = true
            };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);

            Debug.LogFormat("RaiseEventSendReady: {0}", content);
        }

        /// <summary>
        /// Send end signal of simulation to the Server
        /// </summary>
        private void RaiseEventEndSimul()
        {
            Debug.Log("RaiseEventEndSimul");
            byte evCode = EndSimulCode;
            object[] content = new object[] {};

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            };

            SendOptions sendOptions = new SendOptions {
                Reliability = true
            };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);

            Debug.LogFormat("RiaseEventEndSimul: {0}", content);
        }
        
        #endregion


        #region  Private Fields

        /// <summary>
        /// 턴 준비 단계 시작 함수
        /// </summary>
        private void StartReadyTurn()
        {
            Debug.Log("StartReadyTurn");
            this.NowSimul = false;
        }

        /// <summary>
        /// Start Simulating with timetable data
        /// </summary>
        /// <param name="tt">Time Table Data</param>
        private void StartSimul(TimeTable tt)
        {
            this.NowSimul = true;
            Debug.LogFormat("StartTurn: {0}", tt);
        }

        /// <summary>
        /// Modify simulation data with received timetable including player skills
        /// </summary>
        /// <param name="tt">Time Table Data</param>
        private void ModifySimul(TimeTable tt)
        {
            Debug.LogFormat("ModifySimul: {0}", tt);
        }

        #endregion

        #region MonoBehaviourPun CallBacks

        void Awake()
        {
            this.NowSimul = false;
        }

        void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }


        #endregion

    }
}