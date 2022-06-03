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


        #region MonoBehaviour CallBacks

        private void Start()
        {
            // test code
            SimulCanvas.SetActive(false);
        }

        #endregion
    }
}
