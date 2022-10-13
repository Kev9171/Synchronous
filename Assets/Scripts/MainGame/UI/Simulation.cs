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

        #region Private Fields
        [Tooltip("Game data about player and characters")]
        [SerializeField]
        private MainGameData data;

        private ActionData actionData = null;
        private int maxTimeLine;
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

        public void StartSimulationState(ActionData actionData)
        {
            this.actionData = actionData;

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

        
        private TICK_RESULT CheckGameEnd()
        {
            // 0: 계속 진행
            // 1: 무승부
            // 2: MasterClient 승
            // 3: OtherClient 승

            bool a = true;
            bool b = true;
            
            foreach(PlayableCharacter p in data.MyTeamCharacter)
            {
                if (!p.Chara.BreakDown)
                {
                    // not end
                    a = false;
                    break;
                }
            }

            foreach(PlayableCharacter p in data.OtherTeamCharacter)
            {
                if (!p.Chara.BreakDown)
                {
                    // not end
                    b = false;
                    break;
                }
            }

            if (a && b) return TICK_RESULT.DRAW;
            else if (a && !b) return TICK_RESULT.CLIENT_WIN;
            else if (!a && b) return TICK_RESULT.MASTER_WIN;
            else return TICK_RESULT.KEEP_GOING;
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

            StartCoroutine(StartAction(-1));
        }

        IEnumerator StartAction(int time)
        {
            // need codes
            DoAction(time);
            yield return new WaitForSeconds(simulationIntervalSeconds);

            TICK_RESULT result = CheckGameEnd();
            if (result == TICK_RESULT.KEEP_GOING)
            {
                if (time <= maxTimeLine + timeAfterSimul)
                {
                    StartCoroutine(StartAction(++time));
                }
                else
                {
                    SimulationEnd();
                }
            }
            else
            {
                gameEvent.RaiseEventGameEnd(result);
            }
        }

        private void DoAction(int time)
        {
            return;
            /*if (actionData.Data.TryGetValue(time, out var value))
            {
                foreach (object[] d in value)
                {
                    int cid = (int)d[0];

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
            }*/
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
            simulationIntervalSeconds = LogicData.Instance.SimulationIntervalSeconds;
        }

        #endregion
    }
}
