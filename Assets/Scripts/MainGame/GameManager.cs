using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KWY
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject mainCamera;

        [SerializeField]
        private MainGameData data;

        [SerializeField]
        private MainGameEvent gameEvent;

        [SerializeField]
        private ShowNowAction showActions;

        [SerializeField]
        private PlayerMPPanel playerMpPanel;

        [SerializeField]
        private PlayerSkillPanel playerSkillPanel;

        [SerializeField]
        private TurnReady turnReady;

        [SerializeField]
        private Simulation simulation;

        int time = 0;
        int tMax;
        ActionData nowActionData;
        STATE nowState = 0;

        #region Public Methods

        /// <summary>
        /// �÷��̾� MP �� ������ �� �Լ��� ���ؼ� ���� (UI ������Ʈ �����ϴ� �Լ�)
        /// </summary>
        /// <param name="value">������ ��(+, -)</param>
        public void UpdatePlayerMP(int value)
        {
            data.UpdatePlayerMP(value);

            // update UI
            playerMpPanel.UpdateUI();

            if (nowState == STATE.Simul)
            {
                playerSkillPanel.UpdateUI();
            }
        }

        public void SetState(int state, params object[] data)
        {
            switch(state)
            {
                case 0: // turn ready
                    TurnReadyState();
                    nowState = STATE.TurnReady;
                    break;
                case 1: // start simul
                    nowState = STATE.Simul;
                    if (PhotonNetwork.IsMasterClient)
                    {
                        SimulationState();
                        simulation.StartSimulation(new ActionData((Dictionary<int, object[]>)data[0]));
                    } 
                    else
                        SimulationState();
                    break;
                case 2: // game over
                    nowState = STATE.GameOver;
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void TurnReadyState()
        {
            simulation.EndSimulationState();

            // ī�޶� �̵�
            mainCamera.GetComponent<CameraController>().SetCameraTurnReady();

            // mp �߰�
            // player
            if (data.turnNum == 1)
            {
                UpdatePlayerMP((Resources.Load("MainGameLogicData", typeof(LogicData)) as LogicData).playerInitialMp);
            }
            else
            {
                UpdatePlayerMP((Resources.Load("MainGameLogicData", typeof(LogicData)) as LogicData).playerMPIncrement);
            }

            // ���� Ȯ�� �ʿ�
            turnReady.ResetUI();
            turnReady.UpdateUI();
            turnReady.StartTurnReadyState();
        }

        /// <summary>
        /// Set mode from TurnReady to Simul
        /// </summary>
        private void SimulationState()
        {
            turnReady.EndTurnReadyState();

            data.turnNum++;

            mainCamera.GetComponent<CameraController>().SetCameraSimul();

            simulation.UpdateUI();
            simulation.StartSimulationState();
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            data.LoadData();

            turnReady.Init();
            simulation.Init();

            SetState(0);
        }

        #endregion
    }
}
