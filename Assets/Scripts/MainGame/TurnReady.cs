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

            // Ȯ��� ĳ���� ���� ũ��� �ʱ�ȭ �� �ӽ� ��ǥ �ʱ�ȭ
            foreach (PlayableCharacter c in data.MyTeamCharacter)
            {
                c.Chara.ResetTempPos();
                c.CharaObject.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            }

            // ��ư ��� �ʱ�ȭ
            readyBtn.GetComponent<TurnReadyBtn>().ResetReady();

            // �÷��̾� mp �߰�
            data.MyPlayer.AddMp(LogicData.Instance.PlayerMPIncrement);

            // ĳ���� mp �߰�
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
            // 3���� �׼��� ��� �������� ���� ĳ���͸� �̵����� ��ü
            foreach (CID cid in data.CharaActionData.Keys)
            {
                // �� �������� �ʾ��� ��� move�� �߰�
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
            // ĳ���� ���� ���ϰ� + ��ų ���� �г� �Ⱥ��̰�
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
