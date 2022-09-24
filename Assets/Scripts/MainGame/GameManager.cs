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

        ActionData nowActionData;
        STATE nowState = STATE.StandBy;
        

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

        public void SetState(STATE state, params object[] data)
        {
            switch(state)
            {
                case STATE.TurnReady: // turn ready
                    TurnReadyState();
                    nowState = STATE.TurnReady;
                    break;
                case STATE.Simul: // start simul
                    nowState = STATE.Simul;
                    if (PhotonNetwork.IsMasterClient)
                    {
                        SimulationState();
                        simulation.StartSimulation(new ActionData((Dictionary<int, object[]>)data[0]));
                    } 
                    else
                        SimulationState();
                    break;
                case STATE.GameOver: // game over
                    nowState = STATE.GameOver;
                    break;
            }
        }



        private void TurnReadyState()
        {
            simulation.EndSimulationState();

            // ī�޶� �̵�
            mainCamera.GetComponent<CameraController>().SetCameraTurnReady();

            // mp �߰�
            // player
            if (data.turnNum == 1)
            {
                UpdatePlayerMP(LogicData.Instance.PlayerInitialMp);
            }
            else
            {
                UpdatePlayerMP(LogicData.Instance.PlayerMPIncrement);
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


        #region MonoBehaviour CallBacks

        private void Start()
        {
            turnReady.Init();
            simulation.Init();

            SetState(STATE.TurnReady);
        }

        #endregion
    }
}
