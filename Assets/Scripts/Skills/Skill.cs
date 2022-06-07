using UnityEngine;
using System.Collections.Generic;

namespace KWY
{
    public interface ISkill{
        string GetSkillName();

        float GetCost();

        /// <summary>
        /// Insert locations that are affected by its skill based on casting character's location to section list
        /// </summary>
        /// <param name="loc">The vector2 location of casting character</param>
        void SetSection(Vector2 loc);

        /// <summary>
        /// Get a list of sections(Vector2) that are affected by its skill
        /// </summary>
        /// <returns> list of sections(Vector2) that are affected by its skill</returns>
        List<Vector2> GetSection();
    }
}