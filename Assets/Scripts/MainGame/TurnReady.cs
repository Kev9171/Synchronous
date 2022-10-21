//#define TEST

using System.Collections;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

using Photon.Pun;

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

            // ĳ���� ���� �����ϵ���
            characterUIHandler.CharaPanelSelectable = true;
            characterControl.StartControl();

            characterPanel.anchoredPosition = new Vector2(-250, 0);

            // Ȯ��� ĳ���� ���� ũ��� �ʱ�ȭ �� �ӽ� ��ǥ �ʱ�ȭ
            foreach (PlayableCharacter c in data.MyTeamCharacter)
            {
                c.Chara.ResetTempPos();
                c.CharaObject.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            }

            // ��ư ��� �ʱ�ȭ
            readyBtn.GetComponent<TurnReadyBtn>().ResetReady();

            if (PhotonNetwork.IsMasterClient)
            {
                DataController.Instance.AddPlayerMp(LogicData.Instance.PlayerMPIncrement);
                DataController.Instance.AddAllCharactersMp(LogicData.Instance.CharacterMpIncrement);
            }

            /*// �÷��̾� mp �߰�
            data.MyPlayer.AddMp(LogicData.Instance.PlayerMPIncrement);

            // ĳ���� mp �߰�
            foreach(PlayableCharacter pc in data.MyTeamCharacter)
            {
                pc.Chara.AddMP(LogicData.Instance.CharacterMpIncrement);
            }*/

            // show UI
            TurnReadyUI.SetActive(true);

            // start timer
            StartTimer();
        }

        public void EndTurnReadyState()
        {
            // UI �����
            TurnReadyUI.SetActive(false);

            // Ÿ�̸� ����
            ResetTimer();

            // ĳ���� ���� ���ϵ���
            characterUIHandler.CharaPanelSelectable = false;

            // ���õ� ĳ���� ����
            characterControl.SetSelClear();
        }

        public void OnClickTurnReady()
        {
            StopTimer();

            // ĳ���� ���� ����
            characterUIHandler.CharaPanelSelectable = false;

            // ĳ���� �г� �����
            characterUIHandler.HideAllSkillSelPanel();

            FillRandomMoveAtEmpty();

            // event ����
            gameEvent.RaiseEventTurnReady(ActionData.CreateActionData(data.CharaActionData));
        }

        #endregion

        #region Private Methods

        private void FillRandomMoveAtEmpty()
        {
            // 3���� �׼��� ��� �������� ���� ĳ���͸� �̵����� ��ü
            foreach (int id in data.CharaActionData.Keys)
            {
                // �� �������� �ʾ��� ��� move�� �߰�
                if (data.CharaActionData[id].ActionCount != 3)
                {
                    data.CharaActionData[id].ClearActions();
                    for (int i = 0; i < 3; i++)
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
            //PanelBuilder.ShowResultPanel(UICanvasTransform, WINLOSE.WIN, data.CreateResultData());


            // ĳ���� ���� ���ϰ� + ��ų ���� �г� �Ⱥ��̰�
            characterUIHandler.CharaPanelSelectable = false;

            FillRandomMoveAtEmpty();

            ActionData d = ActionData.CreateActionData(data.CharaActionData);
            Debug.Log(d);

#if TEST
            if (!PhotonNetwork.IsMasterClient) return;
            DataController.Instance.ModifyCharacterHp(2, -50);
            DataController.Instance.ModifyCharacterHp(5, -50);
            return;
#endif
            gameEvent.RaiseEventTurnReady(d);
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
