using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class ManageShowingSkills : MonoBehaviour
    {
        [SerializeField]
        GameObject[] selSkillPanels = new GameObject[3];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nth">0 ~ 2</param>
        public void ShowSkillPanel(int nth)
        {
            for (int i=0; i<selSkillPanels.Length; i++)
            {
                selSkillPanels[i].SetActive(nth == i);
            }
        }
    }
}
