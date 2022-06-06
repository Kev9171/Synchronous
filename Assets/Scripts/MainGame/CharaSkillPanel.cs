using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KWY
{
    // attach this at CharaSkillPanel Prefab
    [RequireComponent(typeof(CanvasRenderer))]
    public class CharaSkillPanel : MonoBehaviour
    {
        public Image image;
        public TMP_Text costText;
        public TMP_Text orderText;

        GameObject skillInfoPanel;

        [Tooltip("Info ���µ� �ʿ��� �ּ� Ŭ�� �ð�; move �� ��� ����")]
        public float minClickTime = 1;

        #region Private Fields

        private float clickTime;
        private bool isClick;

        private ActionBase ab;
        
        #endregion

        public void SetValue(ActionBase ab, int order)
        {
            image.sprite = ab.icon;
            costText.text = ab.cost.ToString();
            SetOrder(order);

            this.ab = ab;
        }

        public void SetPanelRef(GameObject skillInfoPanel)
        {
            this.skillInfoPanel = skillInfoPanel;
        }

        public void SetOrder(int order)
        {
            if (order == -1)
            {
                orderText.text = "";
            }
            else
            {
                orderText.text = order.ToString();
            }
        }

        public void OnClickSetOrder()
        {
            Debug.Log("OnClickSetOrder");
        }

        public void ButtonDown()
        {
            isClick = true;
        }
        public void ButtonUp()
        {
            isClick = false;

            if (ab is SkillBase @base && clickTime >= minClickTime)
            {
                skillInfoPanel.GetComponent<SkillInfoPanel>().SetData(@base);
                skillInfoPanel.SetActive(true);
            }
            else
            {
                OnClickSetOrder();
            }
        }

        private void Update()
        {
            if (isClick)
                clickTime += Time.deltaTime;
            else
                clickTime = 0;
        }
    }
}
