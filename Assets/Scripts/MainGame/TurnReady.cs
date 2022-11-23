//#define TEST

using System.Collections;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

using Photon.Pun;
using UI;

namespace KWY
{
    public class TurnReady : MonoBehaviour
    {
        [SerializeField]
        private GameObject TurnReadyUI;

        [SerializeField]
        private TMP_Text timerText;

        [SerializeField]
        private Button readyBtn;

        [Tooltip("Game data about player and characters")]
        [SerializeField]
        private MainGameData data;

        [SerializeField]
        CharacterUIHandler characterUIHandler;

        [SerializeField]
        private MainGameEvent gameEvent;

        [SerializeField]
        private CharacterControl characterControl;

        [SerializeField]
        private RectTransform characterPanel;

        [SerializeField]
        Transform UICanvasTransform;

        private float time;
        private float timeLimit;
        private bool sentData = false;
        private bool timerRunning = false;

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

            // 액션 예약 아이콘 초기화
            characterUIHandler.ClearCharactersActionIcon();

            // 캐릭터 선택 가능하도록
            characterUIHandler.CharaPanelSelectable = true;
            characterControl.StartControl();

            // 캐릭터 정보 표시 패널 이동
            characterPanel.anchoredPosition = new Vector2(-250, 0);

            // 확대된 캐릭터 원래 크기로 초기화 및 임시 좌표 초기화
            foreach (PlayableCharacter c in data.MyTeamCharacter)
            {
                c.Chara.ResetTempPosAndMp();
                c.CharaObject.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            }

            // 버튼 기능 초기화
            GameManager.Instance.TurnReady.ResetReady();

            if (PhotonNetwork.IsMasterClient)
            {
                DataController.Instance.AddPlayerMp(LogicData.Instance.PlayerMPIncrement);
                DataController.Instance.AddAllCharactersMp(LogicData.Instance.CharacterMpIncrement);
            }

            // show UI
            TurnReadyUI.SetActive(true);

            // clear setnData flag
            sentData = false;

            // start timer
            StartTimer();
        }

        public void EndTurnReadyState()
        {
            // UI 숨기기
            TurnReadyUI.SetActive(false);

            // 타이머 리셋
            ResetTimer();

            StopTimer();

            // 캐릭터 선택 못하도록
            characterUIHandler.CharaPanelSelectable = false;

            // 선택된 캐릭터 빼기
            characterControl.SetSelClear();
        }

        public void OnClickTurnReady()
        {
            TimeOut();
        }

        #endregion

        #region Private Methods

        private void FillRandomMoveAtEmpty()
        {
            // 3개의 액션이 모두 정해지지 않은 캐릭터만 이동으로 대체
            foreach (int id in data.CharaActionData.Keys)
            {
                // 선택 안된 부분만 랜덤이동 추가
                for(int i= data.CharaActionData[id].ActionCount; i<3; i++)
                {
                    int dx = 0, dy = 0;
                    while (dx == 0 && dy == 0)
                    {
                        dx = Random.Range(-1, 2);
                        dy = Random.Range(-1, 2);
                    }

                    data.CharaActionData[id].AddMoveAction(ActionType.Move, dx, dy, true, 0, 0);
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
            timerRunning = false;
            StopCoroutine(Timer());
        }

        private void TimeOut()
        {
            // 캐릭터 선택 못하게 + 스킬 선택 패널 안보이게
            characterUIHandler.CharaPanelSelectable = false;

            FillRandomMoveAtEmpty();

            ActionData d = ActionData.CreateActionData(data.CharaActionData);
            Debug.Log(d);

            gameEvent.RaiseEventTurnReady(d);

            sentData = true;
        }

        IEnumerator Timer()
        {
            timerRunning = true;
            while(true)
            {
                if (timerRunning == false) break;
                float t = Mathf.Ceil(timeLimit - time);

                if (t < 0)
                {
                    timerText.text = "0";
                    if (!sentData)
                    {
                        TimeOut();
                    }
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

        #region TurnReadyBtn

        public void SetReady(bool state)
        {
            // 서버로 부터 ok 사인 왔을 떄 호출 
            if (state)
            {
                // 버튼 더 이상 누르지 못하도록
                readyBtn.interactable = false;
            }
            else
            {
                // 오류 보여주기
                PanelBuilder.ShowFadeOutText(UICanvasTransform, "Can not read value from the server...");

                // 다시 원래대로
                readyBtn.interactable = true;
            }
        }

        public void ResetReady()
        {
            // 다시 원래대로
            readyBtn.interactable = true;
        }

        #endregion

        #region MonoBehaviour CallBacks
        private void Awake()
        {
        }
        #endregion
    }
}
