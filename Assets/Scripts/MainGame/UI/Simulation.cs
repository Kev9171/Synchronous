using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private int maxTimeLine;
        private float simulationIntervalSeconds;
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

            StartCoroutine(StartAction(-1));
        }

        IEnumerator StartAction(int time)
        {
            // need codes
            DoAction(time);
            yield return new WaitForSeconds(simulationIntervalSeconds);
            // need codes
            if (time <= maxTimeLine + timeAfterSimul)
            {
                StartCoroutine(StartAction(++time));
            }
            else
            {
                SimulationEnd();
            }
        }

        private void DoAction(int time)
        {
            if (actionData.Data.TryGetValue(time, out var value))
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
            }
        }

        IEnumerator DoCharaMove(int cid, Vector2Int v)
        {
            data.WholeCharacters[cid].MoveTo(v);
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
