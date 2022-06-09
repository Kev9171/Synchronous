using UnityEngine.UI;
using UnityEngine;

namespace KWY
{
    public class UIControlSimul : MonoBehaviour
    {
        #region SimulCanvas Panels

        [SerializeField]
        private GameObject playerSkillPanel;
        [SerializeField]
        private GameObject playerMPPanel;
        [SerializeField]
        private GameObject playerSkillInfoPanel;
        [SerializeField]
        private GameObject settingPanel;
        

        #endregion

        #region Private Fields

        [Tooltip("Game data about player and characters")]
        [SerializeField]
        private MainGameData data;

        #endregion

        #region Public Methods

        public void UpdatePossiblePlayerSKill()
        {
            // 현재 사용 불가능한 플레이어 스킬을 어둡게 처리하여 보여주기 (플레이어 마나 변동 사항이 있을 경우 호출)
        }


        #endregion

        #region Private Methods
        private void LoadPlayerSkills()
        {
            // data.playerskills 에서 값을 가져와서 해당 값에 대한 이미지를 PlayerSkillBtn.Image 에 넣고 보여주기
            playerSkillPanel.GetComponent<PlayerSkillPanel>().SetPanelRef(playerSkillInfoPanel);
            playerSkillPanel.GetComponent<PlayerSkillPanel>().SetData(data.PlayerSkillList);
        }

        
        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            playerSkillInfoPanel.SetActive(false);
            settingPanel.SetActive(false);
            LoadPlayerSkills();

            playerMPPanel.GetComponent<PlayerMPPanel>().SetData(UserManager.UserIcon, data.PlayerMp);
        }

        #endregion

    }
}
