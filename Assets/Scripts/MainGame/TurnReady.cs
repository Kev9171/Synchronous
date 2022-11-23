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

            // �ڽ��� ĳ���� ��ų ���� �гο� ��ġ
            if (PhotonNetwork.IsMasterClient)
            {
                characterUIHandler.InitData(data.CharasTeamA);
            }
            else
            {
                characterUIHandler.InitData(data.CharasTeamB);
            }

            // �ڽ��� ĳ���͸� ���� Ŭ���Ͽ� ���� �����ϵ��� collider �߰�
            foreach (PlayableCharacter c in data.MyTeamCharacter)
            {
                c.Chara.GetComponent<Collider2D>().enabled = true;
            }

            // readybtn�� onclick �߰�
            readyBtn.onClick.AddListener(OnClickTurnReady);
        }

        public void StartTurnReadyState()
        {
            // ��ų ���� �г� ���� �ʱ�ȭ
            characterUIHandler.HideAllSkillSelPanel();

            // �׼� ���� ������ �ʱ�ȭ
            characterUIHandler.ClearCharactersActionIcon();

            // ĳ���� ���� �����ϵ���
            characterUIHandler.CharaPanelSelectable = true;
            characterControl.StartControl();

            // ĳ���� ���� ǥ�� �г� �̵�
            characterPanel.anchoredPosition = new Vector2(-250, 0);

            // Ȯ��� ĳ���� ���� ũ��� �ʱ�ȭ �� �ӽ� ��ǥ �ʱ�ȭ
            foreach (PlayableCharacter c in data.MyTeamCharacter)
            {
                c.Chara.ResetTempPosAndMp();
                c.CharaObject.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            }

            // ��ư ��� �ʱ�ȭ
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
            // UI �����
            TurnReadyUI.SetActive(false);

            // Ÿ�̸� ����
            ResetTimer();

            StopTimer();

            // ĳ���� ���� ���ϵ���
            characterUIHandler.CharaPanelSelectable = false;

            // ���õ� ĳ���� ����
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
            // 3���� �׼��� ��� �������� ���� ĳ���͸� �̵����� ��ü
            foreach (int id in data.CharaActionData.Keys)
            {
                // ���� �ȵ� �κи� �����̵� �߰�
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
            // ĳ���� ���� ���ϰ� + ��ų ���� �г� �Ⱥ��̰�
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
            // ������ ���� ok ���� ���� �� ȣ�� 
            if (state)
            {
                // ��ư �� �̻� ������ ���ϵ���
                readyBtn.interactable = false;
            }
            else
            {
                // ���� �����ֱ�
                PanelBuilder.ShowFadeOutText(UICanvasTransform, "Can not read value from the server...");

                // �ٽ� �������
                readyBtn.interactable = true;
            }
        }

        public void ResetReady()
        {
            // �ٽ� �������
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
