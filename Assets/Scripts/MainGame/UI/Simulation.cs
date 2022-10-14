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
        //private int[] charActNum = new int[data.Characters.Count];
        [Tooltip("�� ���� �ùķ��̼� ���� �� ���� ������� �󸶳� ��� �ϰ� ���� ���ΰ��� ���� int ������ logic data���� �����ϰ� �ִ� interval ����")]
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
                        yield return new WaitForSeconds(SkillManager.GetData((SID)d[2]).triggerTime);
                        StartCoroutine(DoCharaSkill(cid, (SID)d[2], (SkillDicection)d[3], new Vector2Int((int)d[2], (int)d[3])));
                        yield return new WaitForSeconds(SkillManager.GetData((SID)d[2]).castingTime);
                    }
                }
            }
        }

        public void ChangeAction(int cid, int y , ActionBase action)
        {
            if (actionData.Data.TryGetValue(cid, out var value))
            {
                //foreach (object[] d in value)
                //{
                //    int time = (int)d[0];

                //    ActionType t = (ActionType)d[1];

                //    if (t == ActionType.Move)
                //    {
                //        Vector2Int vec = new Vector2Int((int)d[2], (int)d[3]);
                //        List<Vector2Int> des = y % 2 == 0 ? action.areaEvenY : action.areaOddY;
                //        List<Vector2Int> cur = y % 2 != 0 ? action.areaEvenY : action.areaOddY;
                //        int idx = cur.IndexOf(vec);

                //    }
                //}
                for (int i = 0; i < value.Length; i++)
                {
                    object[] d = (object[])value[i];
                    if ((ActionType)d[1] == ActionType.Move)
                    {
                        Vector2Int vec = new Vector2Int((int)d[2], (int)d[3]);
                        List<Vector2Int> v = y != 0 ? action.areaEvenY : action.areaOddY;
                        //List<Vector2Int> cur = y % 2 != 0 ? action.areaEvenY : action.areaOddY;
                        int idx = v.IndexOf(vec);
                        Vector2Int newVec = v[5 - idx] * (-1);
                        Debug.Log("vec = " + vec + ", newvec = " + newVec + "index = " + idx);
                        Debug.Log("ActionType = " + d[1] + ", vec = " + d[2] + ", " + d[3]);
                        d[2] = newVec.x;
                        d[3] = newVec.y;
                        value[i] = d;
                    }
                }
                actionData.Data[cid] = value;
            }
            else
                Debug.Log("no char matching cid");
        }

        IEnumerator DoCharaMove(int cid, Vector2Int v)
        {
            //data.WholeCharacters[cid].MoveTo(v);
            data.WholeCharacters[cid].photonView.RPC("MoveTo", RpcTarget.All, v.x, v.y);
            showActions.ShowMoveLog(cid);
            yield return null;
        }

        IEnumerator DoCharaSkill(int cid, SID sid, SkillDicection dir, Vector2Int v)
        {
            data.WholeCharacters[cid].SpellSkill(sid, dir, v);
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
