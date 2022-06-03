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
        [Tooltip("�� �غ� �ð� ����")]
        private readonly float startTime = 60f;
        private float time;

        [Tooltip("�ʿ� �ִ� �ڽ��� ĳ����(key)�� ���� ��ų ������ ���� �ִ� �ڷᱸ��")]
        private Dictionary<CID, List<SkillBase>> charaSkills = new Dictionary<CID, List<SkillBase>>();

        #endregion

        #region Public Methods

        public void StartTimer()
        {
            // Ÿ�̸� ����
            actTimer = true;
        }

        public void ResetTimer()
        {
            actTimer = false;
            time = startTime;
        }

        public void SetOrderOnActionPanel(AID action, int order) 
        {
            // �ش� �׼�(action) �������� ���� ��ܿ� �ִ� ���ڸ� order�� �ٲٱ�
        }

        public void OnClickTest()
        {
            SelSkillPanel[0].SetActive(true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// UI�� ������ ��ų ������ �����ͼ� �����ϴ� �Լ�
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
