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

        private PlayerSkillPanel _playerSkillPanel;
        private PlayerMPPanel _playerMpPanel;
        

        #endregion

        #region Private Fields

        [Tooltip("Game data about player and characters")]
        [SerializeField]
        private MainGameData data;

        #endregion

        #region Public Methods

        public void Init()
        {
            _playerSkillPanel = playerSkillPanel.GetComponent<PlayerSkillPanel>();
            _playerSkillPanel.SetData(data.PlayerSkillList);

            _playerMpPanel = playerMPPanel.GetComponent<PlayerMPPanel>();

            _playerMpPanel.SetData(UserManager.UserIcon, data.PlayerMp);
        }

        public void UpdateUI()
        {
            _playerSkillPanel.UpdateUI();
            _playerMpPanel.UpdateUI();
        }

        #endregion

        #region Private Methods
        
        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
        }
        

        #endregion

    }
}
