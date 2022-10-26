using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace KWY
{
    class SpawnableSkillResources
    {
        public static string KnightSpecial = "Prefabs/SpawnableSkills/KnightSpecial";
        public static string LighteningVolt = "Prefabs/SpawnableSkills/LightningVolt";

        public static string GetPath(SID sid)
        {
            return sid switch
            {
                SID.FireBall => throw new System.NotImplementedException(),
                SID.LightingVolt => LighteningVolt,
                SID.KnightNormal => throw new System.NotImplementedException(),
                SID.KnightSpecial => KnightSpecial,
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}
