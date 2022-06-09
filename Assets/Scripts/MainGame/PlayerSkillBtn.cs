using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class PlayerSkillBtn : MonoBehaviour
    {
        [SerializeField]
        TMP_Text costLabel;

        [SerializeField]
        Image icon;

        PlayerSkillBase psb;
        private GameObject playerSkillInfoPanel;

        [Tooltip("Info ���µ� �ʿ��� �ּ� Ŭ�� �ð�; move �� ��� ����")]
        public float minClickTime = 1;

        #region Private Fields

        private float clickTime;
        private bool isClick;

        #endregion

        public void SetData(PlayerSkillBase psb)
        {
            costLabel.text = psb.cost.ToString();
            icon.sprite = psb.icon;

            this.psb = psb;
        }

        public void SetPanelRef(GameObject playerSkillInfoPanel)
        {
            this.playerSkillInfoPanel = playerSkillInfoPanel;
        }

        public void OnClickUseSkill()
        {
            Debug.Log("��ų �ߵ�");
        }

        public void ButtonUp()
        {
            isClick = false;

            if (clickTime >= minClickTime)
            {
                playerSkillInfoPanel.GetComponent<PlayerSkillInfoPanel>().SetData(psb);
                playerSkillInfoPanel.SetActive(true);
            }
            else
            {
                OnClickUseSkill();
            }
        }

        public void ButtonDown()
        {
            isClick = true;
        }

        #region MonoBehaviour CallBacks
        private void Update()
        {
            if (isClick)
                clickTime += Time.deltaTime;
            else
                clickTime = 0;
        }
        #endregion

    }
}
