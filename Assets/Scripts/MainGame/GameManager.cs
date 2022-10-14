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
        private PlayerSkillPanel playerSkillPanel;

        [SerializeField]
        private TurnReady turnReady;

        [SerializeField]
        private Simulation simulation;

        [SerializeField]
        Transform UICanvas;

        ActionData nowActionData;

        STATE nowState = STATE.StandBy;

        public void SetState(STATE state, params object[] data)
        {
            switch (state)
            {
                case STATE.TurnReady: // turn ready
                    TurnReadyState();
                    break;
                case STATE.Simul: // start simul
                    SimulationState(
                        new ActionData((Dictionary<int, object[]>)data[0])
                        );
                    break;
                case STATE.GameOver: // game over
                    GameOverState((TICK_RESULT)data[0]);
                    break;
            }
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

        private void SimulationState(ActionData actionData)
        {
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

        // Start is called before the first frame update
        void Start()
        {
            data.LoadData();

            turnReady.Init();
            simulation.Init();




            // must be called at end of Start func.
            SetState(STATE.TurnReady);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}