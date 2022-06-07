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

        [Tooltip("맵에 있는 자신의 캐릭터(key)에 대한 스킬 정보를 갖고 있는 자료구조")]
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


            // 하이라이트를 위한 임시 좌표 초기화
            data.Characters[0].ResetTempPos();
            data.Characters[1].ResetTempPos();
            data.Characters[2].ResetTempPos();
        }

        public void UpdateDataOnUI()
        {
            selCharaPanelManager.GetComponent<ManageShowingSkills>().ShowSkillPanel(-1);

            // 주의: 캐릭터가 쓰러져도 data에는 그대로 남아있어야 함 해당 내용은
            // CharacterPanel의 UpdateData에서 처리
            int idx = 0;
            foreach (Character c in data.Characters)
            {
                characterPanels[idx].GetComponent<CharacterPanel>().UpdateData(c);
                idx++;
            }

            // skill update는 안해도 됨 (사용할 수 있는 스킬이 바뀌거나 하지 않으면)

            playerMPPanel.GetComponent<PlayerMPPanel>().UpdateData(data.PlayerMp);
        }


        public void StartTimer()
        {
            // 타이머 시작
            actTimer = true;
        }

        public void ResetTimer()
        {
            actTimer = false;
            time = data.TimeLimit;
        }

        public void SetOrderOnActionPanel(AID action, int order) 
        {
            // 해당 액션(action) 아이콘의 좌측 상단에 있는 숫자를 order로 바꾸기
        }

        public void OnClickTurnReady()
        {
            SendReadyState();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// UI에 보여줄 스킬 정보를 가져와서 저장하는 함수
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
                // SetPanelRef 를 먼저 호출해야됨
                characterPanels[idx].GetComponent<CharacterPanel>().SetPanelRef(characterInfoPanel, buffInfoPanel);
                characterPanels[idx].GetComponent<CharacterPanel>().SetData(c.Cb, c.Buffs);

                _charaPanels.Add(c.Cb.cid, characterPanels[idx].GetComponent<CharacterPanel>());

                idx++;
            }
        }

        private void SendReadyState()
        {
            Debug.Log("Time is Up!!!");

            /// action 랜덤으로 선택 한 후 aminGameEvent에 넘겨주기


            gameEvent.RaiseEventTurnReady(ActionData.CreateActionData(data.CharaActionData)); // ready 상태 전송 with actiondata
        }

        private IEnumerator TimesUp()
        {
            // 캐릭터 선택 못하게 + 스킬 선택 패널 안보이게
            selCharaPanelManager.GetComponent<ManageShowingSkills>().SetSeletable(false);
            FillRandomMoveAtEmpty();
            SendReadyState();

            yield return null;
        }

        private void FillRandomMoveAtEmpty()
        {
            // 3개의 액션이 모두 정해지지 않은 캐릭터만 이동으로 대체
            foreach (CID cid in data.CharaActionData.Keys)
            {
                Debug.Log(data.CharaActionData[cid].Count);
                // 다 정해지지 않았을 경우 move로 추가
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

                // 잠시 주석 처리
                // SendReadyState();
            }
        }

        #endregion
    }
}
