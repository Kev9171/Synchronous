using UnityEngine;
using System.Collections.Generic;

namespace KWY
{
    public class SFireBall : ISkill
    {
        #region Constant Fields
        const string _SkillName = "FireBall";
        const float _Cost = 1;

        #endregion

        #region Private Fields
        
        string SkillName = _SkillName;
        float Cost = _Cost;
        // Hexagonal Coordinates
        List<Vector2> Section = new List<Vector2>();

        #endregion

        #region ISkill Implementations

        public void SetSection(Vector2 loc)
        {
            // calc the location with parameter, loc and insert them to Section
        }

        public string GetSkillName()
        {
            return this.SkillName;
        }

        public float GetCost()
        {
            return this.Cost;
        }

        public List<Vector2> GetSection()
        {
            return this.Section;
        }
        #endregion
    }
}