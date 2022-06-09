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
            // ���� ��� �Ұ����� �÷��̾� ��ų�� ��Ӱ� ó���Ͽ� �����ֱ� (�÷��̾� ���� ���� ������ ���� ��� ȣ��)
        }


        #endregion

        #region Private Methods
        private void LoadPlayerSkills()
        {
            // data.playerskills ���� ���� �����ͼ� �ش� ���� ���� �̹����� PlayerSkillBtn.Image �� �ְ� �����ֱ�
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
