using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KWY
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject TurnCanvas;

        [SerializeField]
        private GameObject SimulCanvas;

        [SerializeField]
        private GameObject mainCamera;

        [SerializeField]
        private MainGameData data;


        [Tooltip("True �ÿ� �� �غ� ���¸� ������")]
        public bool ShowTurnReadyCanvas = false;
        [Tooltip("True �ÿ� �ùķ��̼� ���¸� ������")]
        public bool ShowSimulCanvas = false;

        public void SetState(int state, params object[] data)
        {
            switch(state)
            {
                case 0: // turn ready
                    TurnReadyState();
                    break;
                case 1: // start simul
                    if (PhotonNetwork.IsMasterClient)
                        Simulation((Dictionary<int, string>)data[0]);
                    else
                        SimulationState();
                    break;
                case 2: // game over
                    break;
            }
        }

        


        

        public void SetCameraOnTurnReady()
        {
            Debug.Log("Move Camera to TurnReady");
            mainCamera.GetComponent<CameraController>().SetCameraTurnReady();
        }

        public void SetCameraOnSimul()
        {
            Debug.Log("Move Camera to Simul");
            mainCamera.GetComponent<CameraController>().SetCameraSimul();
        }




        #region Private Methods

        private void UpdateDataOnUI()
        {

        }

        private void TurnReadyState()
        {
            // ī�޶� �̵�
            SetCameraOnTurnReady();

            // mp �߰�
            // player
            data.mp += data.playerMPIncrement;

        }

        private void SimulationState()
        {
            SetCameraOnSimul();
        }

        private void Simulation(Dictionary<int, string> actionData)
        {

        }
        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            // test code
            SimulCanvas.SetActive(ShowSimulCanvas);
            TurnCanvas.SetActive(ShowTurnReadyCanvas);

            if (ShowTurnReadyCanvas)
            {
                mainCamera.GetComponent<CameraController>().SetCameraTurnReady();
            }
        }

        #endregion
    }
}
