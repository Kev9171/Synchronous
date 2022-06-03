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

        public void SetValue(Sprite sprite, int cost, int order)
        {
            image.sprite = sprite;
            costText.text = cost.ToString();
            SetOrder(order);
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
    }
}

