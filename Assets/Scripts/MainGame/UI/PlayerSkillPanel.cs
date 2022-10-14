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

        [SerializeField]
        private MainGameData data;

        [SerializeField]
        private Color canNotUseSkillColor;

        #region Private Fields

        readonly Dictionary<PSID, GameObject> skillBtns = new Dictionary<PSID, GameObject>();

        #endregion

        #region Public Methods
        
        public void SetData(List<PSID> list)
        {
            foreach (var psid in list)
            {
                PlayerSkillBase psb = PlayerSkillManager.GetData(psid);

                GameObject pskill = Instantiate(playerSkillBtnPrefab, this.transform);
                pskill.GetComponent<PlayerSkillBtn>().SetData(psb);

                skillBtns.Add(psid, pskill);
            }
        }

        #endregion
    }
}
