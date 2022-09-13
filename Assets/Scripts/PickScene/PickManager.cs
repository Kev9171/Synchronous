using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;

namespace KWY
{
    public class PickManager : Singleton<PickManager>
    {
        [SerializeField]
        private GameObject mainCamera;

        //[SerializeField]
        //private MainGameData data;

        [SerializeField]
        private PickSceneData data;

        [SerializeField]
        private PickControl pickControl;

        [SerializeField]
        private MainGameEvent gameEvent;

        [SerializeField]
        private ShowNowAction showActions;

        //[SerializeField]
        //private TurnReady turnReady;

        //[SerializeField]
        //private Simulation simulation;

        [SerializeField] Tilemap hlMap;
        private Color transparent = new Color(1, 1, 1, 0);
        
        [SerializeField] private Text chanceText;
        private int chance;
        public int Chance
        {
            get
            {
                return chance;
            }

            set
            {
                this.chance = value;
                this.chanceText.text = value.ToString();
            }
        }
        public CharacterBtn ClickedBtn { get; private set; }

        int time = 0;
        int tMax;
        ActionData nowActionData;
        STATE nowState = 0;

        #region Public Methods

        //public void SetState(int state, params object[] data)
        //{
        //    switch(state)
        //    {
        //        case 0: // turn ready
        //            TurnReadyState();
        //            nowState = STATE.TurnReady;
        //            break;
        //        case 1: // start simul
        //            nowState = STATE.Simul;
        //            if (PhotonNetwork.IsMasterClient)
        //            {
        //                SimulationState();
        //                //simulation.StartSimulation(new ActionData((Dictionary<int, object[]>)data[0]));
        //            } 
        //            else
        //                SimulationState();
        //            break;
        //        case 2: // game over
        //            nowState = STATE.GameOver;
        //            break;
        //    }
        //}

        public void ClearHighlight()
        {
            //Vector3 pos = new Vector3(0, 0, 0);
            //Vector3Int range;
            //for (float i = -4; i < 5; i += 0.5f)
            //{
            //    for (float j = -6; j < 6; j += 0.7f)
            //    {
            //        pos.x = j;
            //        pos.y = i;
            //        range = hlMap.WorldToCell(pos);
            //        this.hlMap.SetTileFlags(range, TileFlags.None);
            //        this.hlMap.SetColor(range, transparent);
            //    }
            //}

            foreach (var pos in hlMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                Vector3 place = hlMap.CellToWorld(localPlace);
                if (hlMap.HasTile(localPlace))
                {
                    hlMap.SetTileFlags(localPlace, TileFlags.None);
                    hlMap.SetColor(localPlace, transparent);
                }
            }

        }

        public void PickCharacter(CharacterBtn characterBtn)
        {
            this.ClickedBtn = characterBtn;
        }

        public void PickClear()
        {
            ClickedBtn = null;
        }

        public void Timeout()
        {
            PickControl.Instance.RandomDeployCharacter();
            SceneManager.LoadScene("TestScene");
        }

        #endregion

        #region Private Methods

        private void TurnReadyState()
        {
            //simulation.EndSimulationState();

            // 카메라 이동
            //mainCamera.GetComponent<CameraController>().SetCameraTurnReady();

            // 순서 확인 필요
            //turnReady.ResetUI();
            //turnReady.UpdateUI();
            //turnReady.StartTurnReadyState();
        }

        /// <summary>
        /// Set mode from TurnReady to Simul
        /// </summary>
        private void SimulationState()
        {
            //turnReady.EndTurnReadyState();

            data.turnNum++;

            mainCamera.GetComponent<CameraController>().SetCameraSimul();

            //simulation.UpdateUI();
            //simulation.StartSimulationState();
        }

        private void SetNewMaster(Player newMaster)
        {
            PhotonNetwork.SetMasterClient(newMaster);
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            //data.LoadData();
            //SetState(0);
            ClearHighlight();
            Chance = 3;
            SetNewMaster(PhotonNetwork.LocalPlayer);
        }

        private void Update()
        {
            //if (pickControl.deployCounter > 1)
            //{
            //    data.LoadData();
            //}
            
        }

        #endregion
    }
}
