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

        


        [Tooltip("True 시에 턴 준비 상태를 보여줌")]
        public bool ShowTurnReadyCanvas = false;
        [Tooltip("True 시에 시뮬레이션 상태를 보여줌")]
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
                        Simulation(new ActionData((Dictionary<int, object[]>)data[0]));
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

        private void TurnReadyState()
        {
            // 카메라 이동
            SetCameraOnTurnReady();

            // mp 추가
            // player
            data.PlayerMp += data.playerMPIncrement;

        }

        private void SimulationState()
        {
            SetCameraOnSimul();
        }

        private void Simulation(ActionData actionData)
        {
            Debug.Log("simul start -- GameManger");
            nowActionData = actionData;
            StartCoroutine("StartAction", 0);
        }

        int time = 0;
        ActionData nowActionData;

        IEnumerator StartAction(int time)
        {
            DoAction(time);
            yield return new WaitForSeconds(1);
            Debug.Log("StartAction at : " + time);
            StartCoroutine("StartAction", ++time);
        }

        private void DoAction(int time)
        {
            if (nowActionData.Data.TryGetValue(time, out var value))
            {
                foreach(object[] d in value)
                {
                    int cid = (int) d[0];
                    ActionType type = (ActionType)d[1];

                    if (type == ActionType.Move)
                    {
                        StartCoroutine(DoCharaMove(cid, new Vector2Int((int)d[2], (int)d[3])));
                    }
                    else if (type == ActionType.Skill)
                    {
                        StartCoroutine(DoCharaSkill(cid, (SID)d[2], (SkillDicection)d[3]));
                    }
                }
            }
        }

        IEnumerator DoCharaMove(int cid, Vector2Int v)
        {
            data.WholeCharacters[cid].MoveTo(v);
            yield return null;
        }

        IEnumerator DoCharaSkill(int cid, SID sid, SkillDicection dir)
        {
            data.WholeCharacters[cid].SpellSkill(sid, dir);
            yield return null;
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
