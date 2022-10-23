#define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


namespace KWY
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        CameraController cameraController;

        [SerializeField]
        MainGameData data;

        [SerializeField]
        Player player;

        [SerializeField]
        ShowNowAction showActions;

        [SerializeField]
        GameObject loadingScreen;

        [SerializeField]
        private TurnReady turnReady;

        [SerializeField]
        private Simulation simulation;

        [SerializeField]
        Transform UICanvas;

        STATE nowState = STATE.IDLE;

        public static GameManager Instance;
        public TurnReady TurnReady
        {
            get { return turnReady; }
        }
        public Simulation Simulation
        {
            get { return simulation; }
        }

        public void SetState(STATE state, params object[] data)
        {
#if TEST
            Debug.Log($"SetState: {state}");
#endif
            switch (state)
            {
                case STATE.StandBy: // 게임 시작 준비 완료
                    TurnStandBy();
                    break;
                case STATE.TurnReady: // turn ready
                    TurnReadyState();
                    break;
                case STATE.Simul: // start simul
                    if (PhotonNetwork.IsMasterClient)
                    {
                        SimulationState(new ActionData((Dictionary<int, object[]>)data[0]));
                    }
                    else
                    {
                        SimulationState();
                    }
                    break;
                case STATE.GameOver: // game over
                    GameOverState((TICK_RESULT)data[0]);
                    break;
            }
        }

        private void TurnStandBy()
        {
            // UI 관련 데이터 연결
            turnReady.Init();
            simulation.Init();

            SetState(STATE.TurnReady);
            loadingScreen.SetActive(false);
        }

        private void TurnReadyState()
        {
            nowState = STATE.TurnReady;

            // end simulation state
            simulation.EndSimulationState();

            data.TurnNum++;

            // move camera
            cameraController.SetCameraTurnReady();

            // start turn ready state
            turnReady.StartTurnReadyState();
        }

        private void SimulationState()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("SimulationState() must be called only on B-client");
                return;
            }

            nowState = STATE.Simul;

            nowState = STATE.Simul;

            // end turnready state
            turnReady.EndTurnReadyState();

            // move camera
            cameraController.SetCameraSimul();

            // start simulation state
            simulation.StartSimulationState();
        }

        private void SimulationState(ActionData actionData)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("SimulationState() must be called only on master client");
                return;
            }

            nowState = STATE.Simul;

            // end turnready state
            turnReady.EndTurnReadyState();

            // move camera
            cameraController.SetCameraSimul();

            // start simulation state
            simulation.StartSimulationState(actionData);
        }

        private void GameOverState(TICK_RESULT result)
        {
            nowState = STATE.GameOver;
            // TODO

            // 마스터 클라이언트가 이겻을 경우
            if (result == TICK_RESULT.MASTER_WIN)
            {
                PanelBuilder.ShowResultPanel(UICanvas, PhotonNetwork.IsMasterClient ? WINLOSE.WIN : WINLOSE.LOSE, data.CreateResultData());
            }
            // 마스터 클라이언트가 졌을 경우
            else if (result == TICK_RESULT.CLIENT_WIN)
            {
                PanelBuilder.ShowResultPanel(UICanvas, PhotonNetwork.IsMasterClient ? WINLOSE.LOSE : WINLOSE.WIN, data.CreateResultData());
            }
            // 무승부
            else if (result == TICK_RESULT.DRAW)
            {
                PanelBuilder.ShowResultPanel(UICanvas, WINLOSE.DRAW, data.CreateResultData());
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            loadingScreen.SetActive(true);

            data.LoadData();

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}