using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class SelSkillPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject skillPanel;
        [SerializeField]
        private TMP_Text characterNameLabel;
        [SerializeField]
        private GameObject charaSkillPanelPrefab;
        [SerializeField]
        private MoveBase move;


        #region Private Fields

        List<GameObject> skillPanelList = new List<GameObject>();

        #endregion

        #region Public Methods

        public void SetCharaterName(string name)
        {
            characterNameLabel.text = name;
        }

        public void SetOrder(int actionIdx, int order)
        {
            if (actionIdx >= skillPanelList.Count)
            {
                Debug.Log("There is no action idx:" + actionIdx);
            }

            skillPanelList[actionIdx].GetComponent<CharaSkillPanel>().SetOrder(order);
        }

        public void AddSkillPanels(List<SkillBase> list)
        {
            // add move
            GameObject movePanel = Instantiate(charaSkillPanelPrefab, skillPanel.transform);
            movePanel.GetComponent<CharaSkillPanel>().SetValue(move.icon, move.cost, -1);
            skillPanelList.Add(movePanel);

            foreach(SkillBase sb in list)
            {
                GameObject iconPanel = Instantiate(charaSkillPanelPrefab, skillPanel.transform);
                iconPanel.GetComponent<CharaSkillPanel>().SetValue(sb.icon, sb.cost, -1);
                skillPanelList.Add(iconPanel);
            }
        }

        #endregion
    }
}
