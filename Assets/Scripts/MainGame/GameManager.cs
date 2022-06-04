using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


        [Tooltip("True �ÿ� �� �غ� ���¸� ������")]
        public bool ShowTurnReadyCanvas = false;
        [Tooltip("True �ÿ� �ùķ��̼� ���¸� ������")]
        public bool ShowSimulCanvas = false;


        public void SetCameraOnTurnReady()
        {
            mainCamera.GetComponent<CameraController>().SetCameraTurnReady();
        }

        public void SetCameraOnSimul()
        {
            mainCamera.GetComponent<CameraController>().SetCameraSimul();
        }

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
