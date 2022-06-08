using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

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

        [SerializeField]
        private GameObject selCharaPanelManager;

        [SerializeField]
        private Tilemap map;


        #endregion


        #region Private Fields

        [Tooltip("Game data about player and characters")]
        [SerializeField]
        private MainGameData data;

        private bool actTimer = false;
        private float time;
        private bool timesup = false;

        [Tooltip("�ʿ� �ִ� �ڽ��� ĳ����(key)�� ���� ��ų ������ ���� �ִ� �ڷᱸ��")]
        private Dictionary<CID, List<SkillBase>> charaSkills = new Dictionary<CID, List<SkillBase>>();

        private Dictionary<CID, CharacterPanel> _charaPanels = new Dictionary<CID, CharacterPanel>();

        #endregion

        #region Public Fields
        public Dictionary<CID, CharacterPanel> CharaPanels { get { return _charaPanels; } }
        #endregion


        #region Public Methods

        public void UpdateCharaActions(CID cid)
        {
            for (int i = 0; i < data.CharaActionData[cid].Count; i++)
            {
                object[] t = (object[])data.CharaActionData[cid].Actions[i];
                if (ActionType.Move == (ActionType)(t[0]))
                {
                    _charaPanels[cid].SetSelActionImg(i, MoveManager.MoveData.icon);
                }
                else
                {
                    _charaPanels[cid].SetSelActionImg(i, SkillManager.GetData((SID)(t[1])).icon);
                }
            }
        }


        public void StartTurnReady()
        {
            // for test
            data.Characters[0].GetComponent<Collider2D>().enabled = true;
            data.Characters[1].GetComponent<Collider2D>().enabled = true;
            data.Characters[2].GetComponent<Collider2D>().enabled = true;


            // ���̶���Ʈ�� ���� �ӽ� ��ǥ �ʱ�ȭ
            data.Characters[0].ResetTempPos();
            data.Characters[1].ResetTempPos();
            data.Characters[2].ResetTempPos();
        }

        public void UpdateDataOnUI()
        {
            selCharaPanelManager.GetComponent<ManageShowingSkills>().ShowSkillPanel(-1);

            // ����: ĳ���Ͱ� �������� data���� �״�� �����־�� �� �ش� ������
            // CharacterPanel�� UpdateData���� ó��
            int idx = 0;
            foreach (Character c in data.Characters)
            {
                characterPanels[idx].GetComponent<CharacterPanel>().UpdateData(c);
                idx++;
            }

            // skill update�� ���ص� �� (����� �� �ִ� ��ų�� �ٲ�ų� ���� ������)

            playerMPPanel.GetComponent<PlayerMPPanel>().UpdateData(data.PlayerMp);
        }


        public void StartTimer()
        {
            // Ÿ�̸� ����
            actTimer = true;
        }

        public void ResetTimer()
        {
            actTimer = false;
            time = data.TimeLimit;
        }

        public void SetOrderOnActionPanel(AID action, int order) 
        {
            // �ش� �׼�(action) �������� ���� ��ܿ� �ִ� ���ڸ� order�� �ٲٱ�
        }

        public void OnClickTurnReady()
        {
            SendReadyState();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// UI�� ������ ��ų ������ �����ͼ� �����ϴ� �Լ�
        /// </summary>
        private void LoadSkills()
        {
            foreach (Character c in data.Characters)
            {
                List<SkillBase> bases = new List<SkillBase>();
                foreach (SID sid in c.Cb.skills)
                {
                    SkillBase t = SkillManager.GetData(sid);
                    if (t != null)
                    {
                        bases.Add(t);
                    }
                }

                charaSkills.Add(c.Cb.cid, bases);
            }
        }

        private void SetSkillsOnPanel()
        {
            int idx = 0;
            foreach(Character c in data.Characters)
            {
                selSkillPanel[idx].GetComponent<SelSkillPanel>().SetCharaterName(c.Cb.name);
                selSkillPanel[idx].GetComponent<SelSkillPanel>().AddSkillPanels(charaSkills[c.Cb.cid]);
                idx++;
            }
        }

        private void LoadCharacters()
        {
            int idx = 0;
            foreach (Character c in data.Characters)
            {
                // SetPanelRef �� ���� ȣ���ؾߵ�
                characterPanels[idx].GetComponent<CharacterPanel>().SetPanelRef(characterInfoPanel, buffInfoPanel);
                characterPanels[idx].GetComponent<CharacterPanel>().SetData(c.Cb, c.Buffs);

                _charaPanels.Add(c.Cb.cid, characterPanels[idx].GetComponent<CharacterPanel>());

                idx++;
            }
        }

        private void SendReadyState()
        {
            Debug.Log("Time is Up!!!");

            /// action �������� ���� �� �� aminGameEvent�� �Ѱ��ֱ�


            gameEvent.RaiseEventTurnReady(ActionData.CreateActionData(data.CharaActionData)); // ready ���� ���� with actiondata
        }

        private IEnumerator TimesUp()
        {
            // ĳ���� ���� ���ϰ� + ��ų ���� �г� �Ⱥ��̰�
            selCharaPanelManager.GetComponent<ManageShowingSkills>().SetSeletable(false);
            FillRandomMoveAtEmpty();
            SendReadyState();

            yield return null;
        }

        private void FillRandomMoveAtEmpty()
        {
            // 3���� �׼��� ��� �������� ���� ĳ���͸� �̵����� ��ü
            foreach (CID cid in data.CharaActionData.Keys)
            {
                Debug.Log(data.CharaActionData[cid].Count);
                // �� �������� �ʾ��� ��� move�� �߰�
                if (data.CharaActionData[cid].Count != 3)
                {
                    data.CharaActionData[cid].ClearActions();
                    for (int i=0; i<3; i++)
                    {
                        int dx = 0, dy = 0;
                        while (dx == 0 && dy == 0)
                        {
                            dx = Random.Range(-1, 2);
                            dy = Random.Range(-1, 2);
                        }

                        data.CharaActionData[cid].AddMoveAction(ActionType.Move, dx, dy, true);
                    }
                }
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
            time = data.TimeLimit;
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

            playerMPPanel.GetComponent<PlayerMPPanel>().SetData(UserManager.UserIcon, data.PlayerMp);

            StartTimer();

            // for test
            StartTurnReady();
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
                timeText.text = "0";

                StartCoroutine("TimesUp");

                //UpdateDataOnUI(); // test

                // ��� �ּ� ó��
                // SendReadyState();
            }
        }

        #endregion
    }
}
