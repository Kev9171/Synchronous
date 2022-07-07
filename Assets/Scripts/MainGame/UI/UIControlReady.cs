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

        [SerializeField]
        private GameObject[] characterPanels;

        [SerializeField]
        private MainGameEvent gameEvent;

        [SerializeField]
        private GameObject selCharaPanelManager;

        [SerializeField]
        private Tilemap map;

        private PlayerMPPanel _playerMPPanel;
        private SelSkillPanel[] _selSkillPanel = new SelSkillPanel[3];
        private CharacterPanel[] _characterPanels = new CharacterPanel[3];
        private ManageShowingSkills _selCharaPanelManager;

        #endregion


        #region Private Fields

        [Tooltip("Game data about player and characters")]
        [SerializeField]
        private MainGameData data;

        private bool actTimer = false;
        private float time;
        private bool timesup = false;


        private Dictionary<CID, CharacterPanel> _charaPanels = new Dictionary<CID, CharacterPanel>();

        #endregion

        #region Public Fields
        public Dictionary<CID, CharacterPanel> CharaPanels { get { return _charaPanels; } }
        #endregion


        #region Public Methods

        
        public void UpdateUI()
        {
            // 주의: 캐릭터가 쓰러져도 data에는 그대로 남아있어야 함 해당 내용은
            // CharacterPanel의 UpdateData에서 처리
            int idx = 0;
            foreach (Character c in data.Characters)
            {
                _characterPanels[idx++].UpdateUI(c);
            }

            _playerMPPanel.UpdateUI();
        }

        public void ResetUI()
        {
            // 스킬 선택 UI 초기화
            _selCharaPanelManager.ShowSkillPanel(-1);
        }

        public void ShowCharacterActions(CID cid)
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
            ResetTimer();
            StartTimer();

            _selCharaPanelManager.SetSeletable(true);

            // 하이라이트를 위한 임시 좌표 초기화
            data.Characters[0].ResetTempPos();
            data.Characters[1].ResetTempPos();
            data.Characters[2].ResetTempPos();
        }

        public void StartTimer()
        {
            // 타이머 시작
            actTimer = true;
        }

        public void ResetTimer()
        {
            timesup = false;
            actTimer = false;
            time = data.TimeLimit;
        }

        public void OnClickTurnReady()
        {
            SendReadyState();
        }

        /// <summary>
        /// SetActive(false) 되기 전 반드시 호출 필요
        /// </summary>
        public void Init()
        {
            _playerMPPanel = playerMPPanel.GetComponent<PlayerMPPanel>();
            _playerMPPanel.SetData(UserManager.UserIcon, data.PlayerMp);

            _selSkillPanel[0] = selSkillPanel[0].GetComponent<SelSkillPanel>();
            _selSkillPanel[1] = selSkillPanel[1].GetComponent<SelSkillPanel>();
            _selSkillPanel[2] = selSkillPanel[2].GetComponent<SelSkillPanel>();

            _characterPanels[0] = characterPanels[0].GetComponent<CharacterPanel>();
            _characterPanels[1] = characterPanels[1].GetComponent<CharacterPanel>();
            _characterPanels[2] = characterPanels[2].GetComponent<CharacterPanel>();

            _selCharaPanelManager = selCharaPanelManager.GetComponent<ManageShowingSkills>();
            _selCharaPanelManager.ShowSkillPanel(-1);

            LoadCharacters();
            LoadSkills();

            data.Characters[0].GetComponent<Collider2D>().enabled = true;
            data.Characters[1].GetComponent<Collider2D>().enabled = true;
            data.Characters[2].GetComponent<Collider2D>().enabled = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load UI of Characters' skills
        /// </summary>
        private void LoadSkills()
        {
            int idx = 0;
            foreach (Character c in data.Characters)
            {
                _selSkillPanel[idx++].SetData(c.Cb.name, CharaManager.GetData(c.Cb.cid).skills);
            }
        }

        private void LoadCharacters()
        {
            int idx = 0;
            foreach (Character c in data.Characters)
            {
                _characterPanels[idx].SetData(c.Cb, c.Buffs);

                _charaPanels.Add(c.Cb.cid, _characterPanels[idx]);

                idx++;
            }
        }

        private void SendReadyState()
        {
            foreach (CID c in data.CharacterObjects.Keys)
            {
                data.CharacterObjects[c].transform.localScale = new Vector3(0.7f, 0.7f, 1);
            }
            gameEvent.RaiseEventTurnReady(ActionData.CreateActionData(data.CharaActionData)); // ready 상태 전송 with actiondata
        }

        private void TimesUp()
        {
            // 캐릭터 선택 못하게 + 스킬 선택 패널 안보이게
            _selCharaPanelManager.SetSeletable(false);
            FillRandomMoveAtEmpty();
            SendReadyState();
        }

        private void FillRandomMoveAtEmpty()
        {
            // 3개의 액션이 모두 정해지지 않은 캐릭터만 이동으로 대체
            foreach (CID cid in data.CharaActionData.Keys)
            {
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


                TimesUp();
            }
        }

        #endregion
    }
}
