using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class PlayerSkillPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerSkillBtnPrefab;

        #region Private Fields

        private List<PlayerSkillBase> skillList = new List<PlayerSkillBase>();
        GameObject playerSkillInfoPanel;

        #endregion

        #region Private Methods 



        #endregion

        #region Public Methods

        public void SetData(List<PSID> list)
        {
            foreach(var id in list)
            {
                skillList.Add(PlayerSkillManager.GetData(id));
            }

            foreach(var psb in skillList)
            {
                GameObject pskill = Instantiate(playerSkillBtnPrefab, this.transform);
                pskill.GetComponent<PlayerSkillBtn>().SetPanelRef(playerSkillInfoPanel);
                pskill.GetComponent<PlayerSkillBtn>().SetData(psb);

            }
        }

        public void SetPanelRef(GameObject playerSkillInfoPanel)
        {
            this.playerSkillInfoPanel = playerSkillInfoPanel;
        }

        #endregion
    }
}
