using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

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
        private Button SettingBtn;

        [Tooltip("The whole panel of skill selection; the length is equal to initial number of characters = 3")]
        [SerializeField]
        private GameObject[] SelSkillPanel;

        [Tooltip("Ready button")]
        [SerializeField]
        private Button ReadyBtn;

        [Tooltip("Time remaining to ready a turn")]
        [SerializeField]
        private TMP_Text timeText;

        [Tooltip("Player Status Panel - image, mp bar and mp")]
        [SerializeField]
        private GameObject PlayerMPPanel;

        [Tooltip("Panel to show information of a character")]
        [SerializeField]
        private GameObject CharacterInfoPanel;

        [Tooltip("Panel to show information of a skill")]
        [SerializeField]
        private GameObject SkillInfoPanel;

        #endregion

        

        #region Private Fields

        [Tooltip("Game data about player and characters")]
        [SerializeField]
        private MainGameData data;

        private bool actTimer = false;
        [Tooltip("턴 준비 시간 제한")]
        private readonly float startTime = 60f;
        private float time;

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

        public void OnClickTest()
        {
            SelSkillPanel[0].SetActive(true);
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
                        Debug.Log(t);
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
                SelSkillPanel[idx].GetComponent<SelSkillPanel>().SetCharaterName(cb.name);
                SelSkillPanel[idx].GetComponent<SelSkillPanel>().AddSkillPanels(charaSkills[cb.cid]);
                idx++;
            }
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
            LoadSkills();
            Debug.Log(charaSkills.Count); // 1
            SetSkillsOnPanel();

            CharacterInfoPanel.SetActive(false);
            SkillInfoPanel.SetActive(false);

            foreach(var p in SelSkillPanel)
            {
                p.SetActive(false);
            }

            StartTimer();
        }

        private void Update()
        {
            if (actTimer && time > 0)
            {
                time -= Time.deltaTime;
            }
            timeText.text = Mathf.Ceil(time).ToString();
        }

        #endregion
    }
}
