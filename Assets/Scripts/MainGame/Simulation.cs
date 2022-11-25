//#define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KWY
{
    public class Simulation : MonoBehaviour
    {
        [SerializeField] GameObject simulCanvas;


        [SerializeField]
        private PlayerSkillPanel playerSkillPanel;

        [SerializeField]
        private PlayerMPPanel playerMpPanel;

        [SerializeField]
        private ShowNowAction showActions;

        [SerializeField]
        private MainGameEvent gameEvent;

        [SerializeField]
        private RectTransform characterPanel;

        #region Private Fields
        [Tooltip("Game data about player and characters")]
        [SerializeField]
        public MainGameData data;

        public ActionData actionData = null;
        private int maxTimeLine;
        private int finActions;
        private float simulationIntervalSeconds;


        private int validCharaNum = 0;


        Task[] task;

        [Tooltip("이 값은 시뮬레이션 종료 후 다음 진행까지 얼마나 대기 하고 있을 것인가에 대한 int 값으로 logic data에서 지정하고 있는 interval 단위")]
        [SerializeField]
        private int timeAfterSimul = 3;
        #endregion

        #region Public Methods

        public void Init()
        {
            playerSkillPanel.SetData(data.PlayerSkillList);
        }
        public void StartSimulationState()
        {
            characterPanel.anchoredPosition = new Vector2(250, 0);

            foreach (PlayableCharacter p in data.PCharacters.Values)
            {
                p.Chara.ResetTempPosAndMp();
            }

            simulCanvas.SetActive(true);
        }


        public void StartSimulationState(ActionData actionData)
        {
            foreach (PlayableCharacter p in data.PCharacters.Values)
            {
                p.Chara.ResetTempPosAndMp();
            }

            this.actionData = actionData;

            characterPanel.anchoredPosition = new Vector2(250, 0);

            simulCanvas.SetActive(true);

            StartSimulation();
        }

        private void StartSimulation(ActionData actionData)
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
#if TEST
            DataController.Instance.ModifyCharacterHp(0, -200);
            DataController.Instance.ModifyCharacterHp(1, -200);
            DataController.Instance.ModifyCharacterHp(2, -200);
            return;
#endif

            Debug.Log("Simulation starts...");

            maxTimeLine = -1;
            foreach (int t in actionData.Data.Keys)
            {
                maxTimeLine = (t > maxTimeLine) ? t : maxTimeLine;
            }

            //StartCoroutine(StartAction(-1));
            StartAction();
        }

        public void StopAllActions()
        {
            foreach(Task t in task)
            {
                t.Stop();
            }
        }

        void StartAction()
        {
            validCharaNum = MainGameData.Instance.NotBreakDownTeamA + MainGameData.Instance.NotBreakDownTeamB;
            task = new Task[validCharaNum];
            
            int i = 0;
            foreach (int id in data.PCharacters.Keys)
            {
                if (data.PCharacters[id].Chara.BreakDown)
                {
                    continue;
                }

                task[i] = new Task(DoAction(id));
                task[i++].Finished += delegate (bool t) {
                    if (!t) Notify();
                };
            }
        }

        void Notify()
        {
            finActions++;
            if (finActions == validCharaNum)
            {
                finActions = 0;
                foreach (PlayableCharacter p in data.PCharacters.Values)
                {
                    if (p.Chara.BreakDown) continue;
                    p.Chara.SetMoveIdx(0);
                }
                SimulationEnd();
            }
        }

        IEnumerator DoAction(int id)
        {
            if (actionData.Data.TryGetValue(id, out var value))
            {
                foreach (object[] d in value)
                {
                    int time = (int)d[0];

                    ActionType type = (ActionType)d[1];
                    yield return new WaitForSeconds(simulationIntervalSeconds);
                    if (type == ActionType.Move)
                    {
                        StartCoroutine(DoCharaMove(id, new Vector2Int((int)d[2], (int)d[3])));
                        data.PCharacters[id].Chara.SetMoveIdx(1);
                        yield return new WaitForSeconds(simulationIntervalSeconds);
                    }
                    else if (type == ActionType.Skill)
                    {
                        yield return new WaitForSeconds(SkillManager.GetData((SID)d[2]).castingTime);
                        StartCoroutine(DoCharaSkill(id, (SID)d[2], (Direction)d[3], new Vector2Int((int)d[4], (int)d[5])));
                        data.PCharacters[id].Chara.SetMoveIdx(1);
                        yield return new WaitForSeconds(SkillManager.GetData((SID)d[2]).triggerTime);
                    }
                }
            }
        }

        IEnumerator DoCharaMove(int id, Vector2Int v)
        {
            data.PCharacters[id].CharaObject.GetComponent<PhotonView>().RPC("MoveTo", RpcTarget.All, v.x, v.y);
            //data.PCharacters[id].Chara.MoveTo(v.x, v.y);
            showActions.ShowMoveLog(id);
            yield return null;
        }

        IEnumerator DoCharaSkill(int id, SID sid, Direction dir, Vector2Int v)
        {
            data.PCharacters[id].Chara.SpellSkill(sid, dir, v.x, v.y);
            showActions.ShowSkillLog(id, sid);
            yield return null;
        }

        public void ChangeAction(int id, int y)
        {
            if (actionData.Data.TryGetValue(id, out var value))
            {
                int y2 = y;
                for (int i = data.PCharacters[id].Chara.moveIdx; i < value.Length; i++)
                {
                    object[] d = (object[])value[i];
                    if ((ActionType)d[1] == ActionType.Move)
                    {
                        Vector2Int vec = new Vector2Int((int)d[2], (int)d[3]);
                        List<Vector2Int> v = y2 % 2 != 0 ? MoveManager.MoveData.areaEvenY : MoveManager.MoveData.areaOddY;
                        //List<Vector2Int> cur = y % 2 != 0 ? action.areaEvenY : action.areaOddY;
                        int idx = v.IndexOf(vec);
                        Vector2Int newVec = v[5 - idx] * (-1);
                        Debug.Log("vec = " + vec + ", newvec = " + newVec + "index = " + idx);
                        Debug.Log("ActionType = " + d[1] + ", vec = " + d[2] + ", " + d[3]);
                        d[2] = newVec.x;
                        d[3] = newVec.y;
                        value[i] = d;
                        y2 += (int)d[3];
                        Debug.Log("index: " + i + ", value[i] " + value[i] + ", moveIdx: " + data.PCharacters[id].Chara.moveIdx);
                    }
                }
                actionData.Data[id] = value;
            }
            else
                Debug.Log("no char matching cid");
            /*if (actionData.Data.TryGetValue(id, out var value))
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
                for (int i = 0; i < value.Length; i++)
                {
                    Debug.Log(value[0] + ", " + value[1] + ", " + value[2] + ", " + value[3]);
                }
            }*/
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
            simulationIntervalSeconds = LogicData.Instance.SimulationIntervalSeconds;
        }

        private void Update()
        {
        }

        #endregion
    }
}
