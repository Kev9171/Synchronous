using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System.Collections;

using TMPro;

namespace KWY
{
    public class UIControlReady : MonoBehaviour
    {
        #region UI Prefabs
        #endregion

        #region TurnCanvas Elements

        [Tooltip("Setting button")]
        [SerializeField]
        private Button settingBtn;

        [Tooltip("The whole panel of skill selection; the length is equal to initial number of characters = 3")]
        [SerializeField]
        private GameObject[] selSkillPanel;

        [Tooltip("Ready button")]
        [SerializeField]
        private Button readyBtn;

        [Tooltip("Time remaining to ready a turn")]
        [SerializeField]
        private TMP_Text timeText;

        [Tooltip("Player Status Panel - image, mp bar and mp")]
        [SerializeField]
        private GameObject playerMPPanel;

        [Tooltip("Panel to show information of a character")]
        [SerializeField]
        private GameObject characterInfoPanel;

        [Tooltip("Panel to show information of a skill")]
        [SerializeField]
        private GameObject skillInfoPanel;

        [SerializeField]
        private GameObject[] characterPanels;

        [SerializeField]
        private GameObject buffInfoPanel;

        [SerializeField]
        private GameObject settingPanel;

        [SerializeField]
        private MainGameEvent gameEvent;

        #endregion

        

        #region Private Fields

        [Tooltip("Game data about player and characters")]
        [SerializeField]
        private MainGameData data;

        private bool actTimer = false;
        [Tooltip("턴 준비 시간 제한")]
        private readonly float startTime = 60f;
        private float time;
        private bool timesup = false;

        [Tooltip("맵에 있는 자신의 캐릭터(key)에 대한 스킬 정보를 갖고 있는 자료구조")]
        private Dictionary<CID, List<SkillBase>> charaSkills = new Dictionary<CID, List<SkillBase>>();

        #endregion

        #region Public Methods

        public void StartTimer()
        {
            // 타이머 시작
            actTimer = true;
        }

        public void ResetTimer()
        {
            actTimer = false;
            time = startTime;
        }

        public void SetOrderOnActionPanel(AID action, int order) 
        {
            // 해당 액션(action) 아이콘의 좌측 상단에 있는 숫자를 order로 바꾸기
        }

        public void OnClickTurnReady()
        {

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// UI에 보여줄 스킬 정보를 가져와서 저장하는 함수
        /// </summary>
        private void LoadSkills()
        {
            foreach (CharacterBase cb in data.CharacterData)
            {
                List<SkillBase> bases = new List<SkillBase>();
                foreach (SID sid in cb.skills)
                {
                    SkillBase t = SkillManager.GetData(sid);
                    if (t != null)
                    {
                        bases.Add(t);
                    }
                }

                charaSkills.Add(cb.cid, bases);
            }
        }

        private void SetSkillsOnPanel()
        {
            int idx = 0;
            foreach(CharacterBase cb in data.CharacterData)
            {
                selSkillPanel[idx].GetComponent<SelSkillPanel>().SetCharaterName(cb.name);
                selSkillPanel[idx].GetComponent<SelSkillPanel>().AddSkillPanels(charaSkills[cb.cid]);
                idx++;
            }
        }

        private void LoadCharacters()
        {
            int idx = 0;
            foreach (CharacterBase cb in data.CharacterData)
            {
                // SetPanelRef 를 먼저 호출해야됨
                characterPanels[idx].GetComponent<CharacterPanel>().SetPanelRef(characterInfoPanel, buffInfoPanel);
                characterPanels[idx].GetComponent<CharacterPanel>().SetData(cb, data.CharaBuffList(cb.cid));
                idx++;
            }
        }

        private void SendReadyState()
        {
            Debug.Log("Time is Up!!!");

            /// action 랜덤으로 선택 한 후 aminGameEvent에 넘겨주기

            gameEvent.RaiseEventTurnReady(); // ready 상태 전송 with actiondata
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            if (data == null)
            {
                Debug.LogError("Can not find MainGameData in this object");
                return;
            }
            // reset timer
            time = startTime;
        }

        private void Start()
        {

            LoadCharacters();
            LoadSkills();
            SetSkillsOnPanel();

            characterInfoPanel.SetActive(false);
            skillInfoPanel.SetActive(false);
            buffInfoPanel.SetActive(false);
            settingPanel.SetActive(false);

            foreach(var p in selSkillPanel)
            {
                p.SetActive(false);
            }

            StartTimer();
        }

        private void Update()
        {
            if (actTimer)
            {
                time -= Time.deltaTime;
            }
            if (time > 0)
                timeText.text = Mathf.Ceil(time).ToString();
            else if (!timesup)
            {
                timesup = true;
                SendReadyState();
            }
        }

        #endregion
    }
}
