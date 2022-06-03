using UnityEngine.UI;
using UnityEngine;

namespace KWY
{
    public class UIControlSimul : MonoBehaviour
    {
        #region SimulCanvas Panels

        public GameObject PlayerSkillPanel;
        public GameObject LeftSkillLinePanel;
        public GameObject RightSkillLinePanel;
        public GameObject PlayerMPPanel;

        [Tooltip("A bar shows the player's mp")]
        public GameObject PlayerMPBar;
        [Tooltip("The image at top of the screen (on the left of the mp bar")]
        public Image PlayerImage;

        #endregion

        #region Private Fields

        [Tooltip("Game data about player and characters")]
        private MainGameData data;

        #endregion

        #region Public Methods

        public void UpdatePossiblePlayerSKill()
        {
            // ���� ��� �Ұ����� �÷��̾� ��ų�� ��Ӱ� ó���Ͽ� �����ֱ� (�÷��̾� ���� ���� ������ ���� ��� ȣ��)
        }

        // ������(?)�� ����Ǿ� �ϴ� �Լ�
        public void ShowAction(AID action, int characterIndex, bool left)
        {
            // �ùķ��̼� �� ����Ǵ� action���� ȭ�鿡 �����ֱ� (parent panel�� ���� �Ӽ��� �ֱ⶧���� SKillLinePanel�� �θ�� ������ �ϸ� �ɵ�? - Ȯ�� �ʿ�)

            // left
            if (left)
            {
                // make object with action, characterIndex

                // set parent to LeftSkillLinePanel
            }
            // right
            else
            {
                // make object with action, characterIndex

                // set parent to RightSkillLinePanel
            }

        }

        #endregion

        #region Private Methods
        private void ShowPlayerSkill()
        {
            // data.playerskills ���� ���� �����ͼ� �ش� ���� ���� �̹����� PlayerSkillBtn.Image �� �ְ� �����ֱ�
        }

        
        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            data = GetComponent<MainGameData>();

            if (data == null)
            {
                Debug.LogError("Can not find MainGameData in this object");
            }
        }

        #endregion

    }
}
