using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class SkillInfoPanel : MonoBehaviour
    {
        [SerializeField]
        Image skillIcon;

        [SerializeField]
        TMP_Text skillNameLabel;

        [SerializeField]
        TMP_Text casterNameLabel;

        [SerializeField]
        TMP_Text costLabel;

        [SerializeField]
        TMP_Text skillExLabel;

        [SerializeField]
        Image skillImg;

        [SerializeField]
        GameObject containerPanel;

        public void SetData(SkillBase sb)
        {
            skillIcon.sprite = sb.icon;
            skillNameLabel.text = sb.name;
            string t = "";
            foreach(CID cid in sb.casters)
            {
                t += " " + cid;
            }
            casterNameLabel.text = t;
            costLabel.text = "cost: " + sb.cost.ToString();
            skillExLabel.text = sb.skillExplanation;
            skillImg.sprite = sb.skillExImg;
        }

        public void OnClickClose()
        {
            containerPanel.SetActive(false);
        }
    }
}
