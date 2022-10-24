using TMPro;
using UnityEngine;
using System.Collections.Generic;

using KWY;

namespace UI
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

        public void SetData(string name, List<SID> sList) 
        {
            characterNameLabel.text = name;

            // add move
            GameObject movePanel = Instantiate(charaSkillPanelPrefab, skillPanel.transform);
            movePanel.GetComponent<CharaSkillPanel>().SetData(move);
            skillPanelList.Add(movePanel);

            foreach (var sid in sList)
            {
                SkillBase sb = SkillManager.GetData(sid);

                GameObject iconPanel = Instantiate(charaSkillPanelPrefab, skillPanel.transform);
                iconPanel.GetComponent<CharaSkillPanel>().SetData(sb);
                skillPanelList.Add(iconPanel);
            }
        }

        #endregion
    }
}
