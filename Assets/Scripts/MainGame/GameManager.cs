using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        CameraController cameraController;

        [SerializeField]
        MainGameData data;

        [SerializeField]
        ShowNowAction showActions;

        [SerializeField]
        private PlayerMPPanel playerMpPanel;

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
                    nowState = STATE.TurnReady;
                    TurnReadyState();
                    break;
                case STATE.Simul: // start simul
                    nowState = STATE.Simul;
                    SimulationState(
                        new ActionData((Dictionary<int, object[]>)data[0])
                        );
                    break;
                case STATE.GameOver: // game over
                    nowState = STATE.GameOver;
                    GameOverState();
                    break;
            }
        }

        private void TurnReadyState()
        {
            // end simulation state
            simulation.EndSimulationState();

            data.turnNum++;

            // move camera
            cameraController.SetCameraTurnReady();

            // start turn ready state
            turnReady.StartTurnReadyState();
        }

        private void SimulationState(ActionData actionData)
        {
            // end turnready state
            turnReady.EndTurnReadyState();

            // move camera
            cameraController.SetCameraSimul();

            // start simulation state
            simulation.StartSimulationState(actionData);
        }

        private void GameOverState()
        {

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