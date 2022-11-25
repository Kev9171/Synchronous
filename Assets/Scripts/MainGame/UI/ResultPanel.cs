using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;

using TMPro;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class ResultPanel : MonoBehaviourPunCallbacks, IInstantiatableUI
    {
        [SerializeField]
        TMP_Text resultText;

        [SerializeField]
        TMP_Text scoreText;

        [SerializeField]
        Button okBtn;

        [SerializeField]
        GameObject scoreListPanel;

        // юс╫ц╥н Object
        public void SetData(WINLOSE result, ResultData data)
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

            int totalScore = 0;

            foreach (PlayableCharacter p in data.MyTeamCharacters)
            {
                GameObject list = Instantiate(
                    Resources.Load(
                        "Prefabs/UI/Game/ScoreListPanel",
                        typeof(GameObject)), scoreListPanel.transform) as GameObject;

                int score = GetCharacterScore(p.Chara.Hp, p.Chara.MaxHp);
                totalScore += score;

                list.GetComponent<ScoreListPanel>().SetData(p.Chara.Cb.name + " bonus", score);
            }

            {
                int score = GetPlayerSkillCountScore(data.PlayerSkillCount);
                GameObject t = Instantiate(
                Resources.Load(
                    "Prefabs/UI/Game/ScoreListPanel",
                    typeof(GameObject)), scoreListPanel.transform) as GameObject;
                t.GetComponent<ScoreListPanel>().SetData("PlayerSkill", score);
                totalScore += score;
            }

            scoreText.text = totalScore.ToString();
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
    }
}
