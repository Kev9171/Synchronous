using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;

using TMPro;
using System.Collections.Generic;
using System.Collections;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class ResultPanel : MonoBehaviourPunCallbacks, IInstantiatableUI
    {
        [SerializeField]
        List<GameObject> leftCharaPanel; // must be 3

        [SerializeField]
        List<Image> leftCharaSprite;

        [SerializeField]
        List<TMP_Text> leftCharaNameLabel;

        [SerializeField]
        List<TMP_Text> leftCharaHpLabel;

        [SerializeField]
        GameObject leftPlayerSkillCountPanel;

        [SerializeField]
        TMP_Text leftPlayerSkillCountLabel;

        [SerializeField]
        GameObject scoreListPanel;

        [SerializeField]
        GameObject scoreListBox;

        [SerializeField]
        GameObject totalScorePanel;

        [SerializeField]
        TMP_Text resultText;

        [SerializeField]
        TMP_Text scoreText;

        [SerializeField]
        Button okBtn;


        ResultData data;
        WINLOSE result;
        private int totalScore = 0;
        private readonly int[] chScore = new int[3];
        private int psScore = 0;

        // юс╫ц╥н Object
        public void SetData(WINLOSE result, ResultData data)
        {
            this.data = data;
            this.result = result;

            totalScore = 0;

            int i = 0;
            foreach (PlayableCharacter p in data.MyTeamCharacters)
            {
                chScore[i] = GetCharacterScore(p.Chara.Hp, p.Chara.MaxHp);
                totalScore += chScore[i];
                i++;
            }

            {
                psScore = GetPlayerSkillCountScore(data.PlayerSkillCount);
                GameObject t = Instantiate(
                Resources.Load(
                    "Prefabs/UI/Game/ScoreListPanel",
                    typeof(GameObject)), scoreListBox.transform) as GameObject;
                t.GetComponent<ScoreListPanel>().SetData("PlayerSkill", psScore);
                totalScore += psScore;
            }

            okBtn.interactable = false;
            StartCoroutine(ShowResult());
            okBtn.interactable = true;
        }

        public void Init()
        {
            //
        }

        public void OnClickClose()
        {
            // Leave the room 
            if (PhotonNetwork.LeaveRoom())
            {
                Debug.Log("Leave the room...");
            }
            else
            {
                Debug.Log("Can not leave the room");
            }

        }


        private int GetCharacterScore(float hp, float maxHp)
        {
            if (hp == 0) return 0;
            return (int)(LogicData.Instance.CharacterScoreMultiplier * (hp / maxHp));
        }

        private int GetPlayerSkillCountScore(int count)
        {
            return LogicData.Instance.PlayerSkillCountScoreMultiplier * count;
        }

        private void Start()
        {
            Init();
            okBtn.onClick.AddListener(OnClickClose);
        }

        public override void OnLeftRoom()
        {
            if (SceneManager.GetActiveScene().name == "MainGameScene")
            {
                // load the start scene
                SceneManager.LoadScene("StartScene");
            }

            base.OnLeftRoom();
        }

        IEnumerator ShowResult()
        {
            {
                switch (result)
                {
                    case WINLOSE.WIN:
                        resultText.text = "WIN";
                        break;
                    case WINLOSE.LOSE:
                        resultText.text = "LOST";
                        break;
                    case WINLOSE.DRAW:
                        resultText.text = "DRAW";
                        break;
                }
                yield return new WaitForSeconds(0.2f);
            }

            {
                int i = 0;
                foreach (PlayableCharacter pc in data.MyTeamCharacters)
                {
                    leftCharaSprite[i].sprite = pc.Chara.Cb.icon;
                    leftCharaNameLabel[i].text = pc.Chara.Cb.name;
                    leftCharaHpLabel[i].text = $"{pc.Chara.Hp}/{pc.Chara.MaxHp}";

                    leftCharaPanel[i].SetActive(true);

                    ++i;

                    yield return new WaitForSeconds(0.2f);
                }

                leftPlayerSkillCountLabel.text = data.PlayerSkillCount.ToString();
                leftPlayerSkillCountPanel.SetActive(true);
                yield return new WaitForSeconds(0.2f);
            }

            scoreListPanel.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            {
                int i = 0;
                foreach (PlayableCharacter p in data.MyTeamCharacters)
                {
                    GameObject list = Instantiate(
                        Resources.Load(
                            "Prefabs/UI/Game/ScoreListPanel",
                            typeof(GameObject)), scoreListBox.transform) as GameObject;

                    list.GetComponent<ScoreListPanel>().SetData(p.Chara.Cb.name + " bonus", chScore[i]);
                    ++i;

                    yield return new WaitForSeconds(0.3f);
                }

                GameObject t = Instantiate(
                Resources.Load(
                    "Prefabs/UI/Game/ScoreListPanel",
                    typeof(GameObject)), scoreListBox.transform) as GameObject;
                t.GetComponent<ScoreListPanel>().SetData("PlayerSkill", psScore);
            }

            yield return new WaitForSeconds(0.5f);
            scoreText.text = totalScore.ToString();
            totalScorePanel.SetActive(true);
        }
    }
}
