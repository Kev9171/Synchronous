using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                    GameOverState((Team)data[0]);
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

        private void GameOverState(Team winTeam)
        {
            nowState = STATE.GameOver;
            // TODO
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