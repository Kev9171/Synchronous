using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

using Photon.Pun;

namespace KWY
{
    public class TurnReady : MonoBehaviour
    {
        #region Canvas Elements

        [SerializeField]
        private GameObject TurnReadyUI;

        [SerializeField]
        private TMP_Text timerText;

        [SerializeField]
        private Button readyBtn;
        #endregion

        #region Private Fields

        [Tooltip("Game data about player and characters")]
        [SerializeField]
        private MainGameData data;

        [SerializeField]
        private ManageShowingSkills selCharaPanelManager;

        [SerializeField]
        private SelSkillPanel[] selSkillPanels = new SelSkillPanel[3];

        [SerializeField]
        private CharacterPanel[] characterPanels = new CharacterPanel[3];

        [SerializeField]
        private PlayerMPPanel playerMpPanel;

        [SerializeField]
        private MainGameEvent gameEvent;

        [SerializeField]
        private CharacterControl characterControl;

        private Dictionary<CID, CharacterPanel> charaPanels = new Dictionary<CID, CharacterPanel>();

        private float time;
        private float timeLimit;

        #endregion

        #region Public Methods

        public void Init()
        {
            timeLimit = LogicData.Instance.TimeLimit;
            ResetUI();

            int idx = 0;
            foreach (Character c in data.Characters)
            {
                selSkillPanels[idx++].SetData(c.Cb.name, CharaManager.GetData(c.Cb.cid).skills);
            }

            idx = 0;
            foreach (Character c in data.Characters)
            {
                characterPanels[idx].SetData(c.Cb, c.Buffs);
                charaPanels.Add(c.Cb.cid, characterPanels[idx]);
                idx++;
            }

            data.Characters[0].GetComponent<Collider2D>().enabled = true;
            data.Characters[1].GetComponent<Collider2D>().enabled = true;
            data.Characters[2].GetComponent<Collider2D>().enabled = true;
        }

        public void UpdateUI()
        {
            // 각 캐릭터 정보에 따라 캐릭터 정보 표시 UI 업데이트
            int idx = 0;
            foreach (Character c in data.Characters)
            {
                characterPanels[idx++].UpdateUI(c);
            }

            // Update Mp Panel
            playerMpPanel.UpdateUI();
        }

        public void ResetUI()
        {
            // 스킬 선택 UI 초기화
            selCharaPanelManager.ShowSkillPanel(-1);

            // 확대된 캐릭터 원래 크기로 초기화
            foreach (CID c in data.CharacterObjects.Keys)
            {
                data.CharacterObjects[c].transform.localScale = new Vector3(0.7f, 0.7f, 1);
            }

            // 하이라이트를 위한 임시 좌표 초기화
            data.Characters[0].ResetTempPos();
            data.Characters[1].ResetTempPos();
            data.Characters[2].ResetTempPos();

            readyBtn.GetComponent<TurnReadyBtn>().ResetReady();
        }

        public void StartTurnReadyState()
        {
            // show UI
            TurnReadyUI.SetActive(true);

            // 캐릭터 선택 가능하도록
            selCharaPanelManager.SetSeletable(true);
            characterControl.StartControl();

            // start timer
            StartTimer();
        }

        public void EndTurnReadyState()
        {
            TurnReadyUI.SetActive(false);
            ResetTimer();
            selCharaPanelManager.SetSeletable(false);
            characterControl.SetSelClear();
        }

        public void OnClickTurnReady()
        {
            StopTimer();

            // 캐릭터 선택 못하게 + 스킬 선택 패널 안보이게
            selCharaPanelManager.SetSeletable(false);

            FillRandomMoveAtEmpty();
            gameEvent.RaiseEventTurnReady(ActionData.CreateActionData(data.CharaActionData));

            // for test
            // 테스트로 masterclient 가 승리가 되도록
            /*if (PhotonNetwork.IsMasterClient)
                gameEvent.RaiseEventGameEnd();*/
        }

        public void ShowCharacterActionPanel(CID cid)
        {
            for (int i = 0; i < data.CharaActionData[cid].Count; i++)
            {
                object[] t = (object[])data.CharaActionData[cid].Actions[i];
                if (ActionType.Move == (ActionType)(t[0]))
                {
                    charaPanels[cid].SetSelActionImg(i, MoveManager.MoveData.icon);
                }
                else
                {
                    charaPanels[cid].SetSelActionImg(i, SkillManager.GetData((SID)(t[1])).icon);
                }
            }
        }

        #endregion

        #region Private Methods

        private void FillRandomMoveAtEmpty()
        {
            // 3개의 액션이 모두 정해지지 않은 캐릭터만 이동으로 대체
            foreach (CID cid in data.CharaActionData.Keys)
            {
                // 다 정해지지 않았을 경우 move로 추가
                if (data.CharaActionData[cid].Count != 3)
                {
                    data.CharaActionData[cid].ClearActions();
                    for (int i = 0; i < 3; i++)
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

        #region Timer

        private void StartTimer()
        {
            time = 0;
            StartCoroutine(Timer());
        }

        private void ResetTimer()
        {
            time = 0;
        }

        private void StopTimer()
        {
            StopCoroutine(Timer());
        }

        private void TimeOut()
        {
            // 캐릭터 선택 못하게 + 스킬 선택 패널 안보이게
            selCharaPanelManager.SetSeletable(false);

            FillRandomMoveAtEmpty();
            gameEvent.RaiseEventTurnReady(ActionData.CreateActionData(data.CharaActionData));
        }

        IEnumerator Timer()
        {
            while(true)
            {
                float t = Mathf.Ceil(timeLimit - time);

                if (t < 0)
                {
                    timerText.text = "0";
                    TimeOut();
                    break;
                }
                else
                {
                    timerText.text = t.ToString();
                    time += 0.5f;
                    yield return new WaitForSeconds(0.5f);
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
        }
        #endregion
    }
}
