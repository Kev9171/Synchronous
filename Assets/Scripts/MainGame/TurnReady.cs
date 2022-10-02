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
        CharacterUIHandler characterUIHandler;

        [SerializeField]
        private MainGameEvent gameEvent;

        [SerializeField]
        private CharacterControl characterControl;

        private float time;
        private float timeLimit;

        #endregion

        #region Public Methods

        public void Init()
        {
            timeLimit = LogicData.Instance.TimeLimit;

            // 자신의 캐릭터 스킬 선택 패널에 배치
            if (PhotonNetwork.IsMasterClient)
            {
                characterUIHandler.InitData(data.CharasTeamA);
            }
            else
            {
                characterUIHandler.InitData(data.CharasTeamB);
            }

            // 자신의 캐릭터를 직접 클릭하여 선택 가능하도록 collider 추가
            foreach (PlayableCharacter c in data.MyTeamCharacter)
            {
                c.Chara.GetComponent<Collider2D>().enabled = true;
            }

            // readybtn에 onclick 추가
            readyBtn.onClick.AddListener(OnClickTurnReady);
        }

        public void StartTurnReadyState()
        {
            // 스킬 선택 패널 전부 초기화
            characterUIHandler.HideAllSkillSelPanel();

            // 캐릭터 선택 가능하도록
            characterUIHandler.CharaPanelSelectable = true;
            characterControl.StartControl();

            // 확대된 캐릭터 원래 크기로 초기화 및 임시 좌표 초기화
            foreach (PlayableCharacter c in data.MyTeamCharacter)
            {
                c.Chara.ResetTempPos();
                c.CharaObject.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            }

            // 버튼 기능 초기화
            readyBtn.GetComponent<TurnReadyBtn>().ResetReady();

            // 플레이어 mp 추가
            data.MyPlayer.AddMp(LogicData.Instance.PlayerMPIncrement);

            // 캐릭터 mp 추가
            foreach(PlayableCharacter pc in data.MyTeamCharacter)
            {
                pc.Chara.AddMP(LogicData.Instance.CharacterMpIncrement);
            }

            // show UI
            TurnReadyUI.SetActive(true);            

            // start timer
            StartTimer();
        }

        public void EndTurnReadyState()
        {
            // UI 숨기기
            TurnReadyUI.SetActive(false);

            // 타이머 리셋
            ResetTimer();

            // 캐릭터 선택 못하도록
            characterUIHandler.CharaPanelSelectable = false;

            // 선택된 캐릭터 빼기
            characterControl.SetSelClear();
        }

        public void OnClickTurnReady()
        {
            StopTimer();

            // 캐릭터 선택 막고
            characterUIHandler.CharaPanelSelectable = false;

            // 캐릭터 패널 숨기기
            characterUIHandler.HideAllSkillSelPanel();

            FillRandomMoveAtEmpty();

            // event 전송
            gameEvent.RaiseEventTurnReady(ActionData.CreateActionData(data.CharaActionData));
        }

        public void ShowCharacterActionPanel(CID cid)
        {
            for (int i = 0; i < data.CharaActionData[cid].Count; i++)
            {
                /*object[] t = (object[])data.CharaActionData[cid].Actions[i];
                if (ActionType.Move == (ActionType)(t[0]))
                {
                    charaPanels[cid].SetSelActionImg(i, MoveManager.MoveData.icon);
                }
                else
                {
                    charaPanels[cid].SetSelActionImg(i, SkillManager.GetData((SID)(t[1])).icon);
                }*/
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
            characterUIHandler.CharaPanelSelectable = false;

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
        }
        #endregion
    }
}
