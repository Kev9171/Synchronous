using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KWY
{
    public class Simulation : MonoBehaviour
    {
        #region Canvas Elements

        [SerializeField] GameObject simulCanvas;

        #endregion

        #region Private Fields

        [SerializeField]
        private PlayerSkillPanel playerSkillPanel;

        [SerializeField]
        private PlayerMPPanel playerMpPanel;

        [SerializeField]
        private ShowNowAction showActions;

        [SerializeField]
        private MainGameEvent gameEvent;

        #endregion

        #region Private Fields
        [Tooltip("Game data about player and characters")]
        [SerializeField]
        private MainGameData data;

        private ActionData actionData = null;
        private int maxTimeLine, finActions;
        private float simulationIntervalSeconds;
        [Tooltip("이 값은 시뮬레이션 종료 후 다음 진행까지 얼마나 대기 하고 있을 것인가에 대한 int 값으로 logic data에서 지정하고 있는 interval 단위")]
        [SerializeField]
        private int timeAfterSimul = 3;
        #endregion

        #region Public Methods

        public void Init()
        {
            playerSkillPanel.SetData(data.PlayerSkillList);
        }

        public void UpdateUI()
        {
            playerSkillPanel.UpdateUI();
        }

        public void StartSimulationState()
        {
            simulCanvas.SetActive(true);
        }

        public void StartSimulation(ActionData actionData)
        {
            this.actionData = actionData;

            StartSimulation();
        }

        public void EndSimulationState()
        {
            simulCanvas.SetActive(false);
        }

        #endregion

        #region Simulation 

        private void StartSimulation()
        {
            Debug.Log("Simulation starts...");

            maxTimeLine = -1;
            foreach (int t in actionData.Data.Keys)
            {
                maxTimeLine = (t > maxTimeLine) ? t : maxTimeLine;
            }
            foreach(Character ch in data.Characters)
            {
                ch.ResetTempPos();
            }

            //StartCoroutine(StartAction(-1));
            StartAction();
        }

        //IEnumerator StartAction(int time)
        //{
        //    // need codes
        //    DoAction(time);
        //    yield return new WaitForSeconds(simulationIntervalSeconds);
        //    // need codes
        //    if (time <= maxTimeLine + timeAfterSimul)
        //    {
        //        StartCoroutine(StartAction(++time));
        //    }
        //    else
        //    {
        //        SimulationEnd();
        //    }
        //}
        void StartAction()
        {
            Task[] task = new Task[data.Characters.Count];
            int i = 0;
            foreach (Character ch in data.Characters)
            {
                Debug.Log("action start");
                task[i] = new Task(DoAction((int)ch.Cb.cid));
                task[i++].Finished += delegate (bool t) {
                    if (!t) Notify();
                };
                //StartCoroutine(DoAction((int)ch.Cb.cid));
            }

        }
        void Notify()
        {
            finActions++;
            if(finActions == data.Characters.Count)
            {
                finActions = 0;
                SimulationEnd();
            }
        }

        //private void DoAction(int time)
        //{
        //    if (actionData.Data.TryGetValue(time, out var value))
        //    {
        //        foreach (object[] d in value)
        //        {
        //            int cid = (int)d[0];

        //            ActionType type = (ActionType)d[1];

        //            if (type == ActionType.Move)
        //            {
        //                StartCoroutine(DoCharaMove(cid, new Vector2Int((int)d[2], (int)d[3])));
        //            }
        //            else if (type == ActionType.Skill)
        //            {
        //                StartCoroutine(DoCharaSkill(cid, (SID)d[2], (SkillDicection)d[3]));
        //            }
        //        }
        //    }
        //}

        IEnumerator DoAction(int cid)
        {
            if (actionData.Data.TryGetValue(cid, out var value))
            {
                foreach (object[] d in value)
                {
                    int time = (int)d[0];

                    ActionType type = (ActionType)d[1];
                    yield return new WaitForSeconds(simulationIntervalSeconds);
                    if (type == ActionType.Move)
                    {
                        StartCoroutine(DoCharaMove(cid, new Vector2Int((int)d[2], (int)d[3])));
                        yield return new WaitForSeconds(simulationIntervalSeconds);
                    }
                    else if (type == ActionType.Skill)
                    {
                        yield return new WaitForSeconds(SkillManager.GetData((SID)d[1]).triggerTime);
                        StartCoroutine(DoCharaSkill(cid, (SID)d[2], (SkillDicection)d[3]));
                        yield return new WaitForSeconds(SkillManager.GetData((SID)d[1]).castingTime);
                    }
                }
            }
        }

        public void ChangeAction(int cid, int y , ActionBase action)
        {
            if (actionData.Data.TryGetValue(cid, out var value))
            {
                foreach (object[] d in value)
                {
                    int time = (int)d[0];

                    ActionType t = (ActionType)d[1];

                    if (t == ActionType.Move)
                    {
                        Vector2Int vec = new Vector2Int((int)d[2], (int)d[3]);
                        List<Vector2Int> des = y % 2 == 0 ? action.areaEvenY : action.areaOddY;
                        List<Vector2Int> cur = y % 2 != 0 ? action.areaEvenY : action.areaOddY;
                        int idx = cur.IndexOf(vec);
                        
                    }

                }
                for(int i = 0; i < value.Length; i++)
                {
                    Debug.Log(value[0] + ", " + value[1] + ", " + value[2] + ", " + value[3]);
                }
            }
        }

        IEnumerator DoCharaMove(int cid, Vector2Int v)
        {
            //data.WholeCharacters[cid].MoveTo(v);
            data.WholeCharacters[cid].photonView.RPC("MoveTo", RpcTarget.All, v.x, v.y);
            showActions.ShowMoveLog(cid);
            yield return null;
        }

        IEnumerator DoCharaSkill(int cid, SID sid, SkillDicection dir)
        {
            data.WholeCharacters[cid].SpellSkill(sid, dir);
            showActions.ShowSkillLog(cid, sid);
            yield return null;
        }

        /// <summary>
        /// Called when the simulation ended
        /// </summary>
        private void SimulationEnd()
        {
            gameEvent.RaiseEventSimulEnd();
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            LogicData logicData = Resources.Load(
                "MainGameLogicData", typeof(LogicData)) as LogicData;

            if (logicData == null)
            {
                Debug.LogError("Can not find LogicData at 'Resources/MainGameLogicData");
                return;
            }

            simulationIntervalSeconds = logicData.simulationIntervalSeconds;
        }

        #endregion
    }
}
