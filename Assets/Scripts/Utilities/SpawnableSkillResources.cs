using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace KWY
{
    class SpawnableSkillResources
    {
        public static string FireBall = "Prefabs/SpawnableSkills/FireBall";
        public static string KnightSpecial = "Prefabs/SpawnableSkills/KnightSpecial";
        public static string LighteningVolt = "Prefabs/SpawnableSkills/LightningVolt";

        public static string GetPath(SID sid)
        {
            return sid switch
            {
                SID.FireBall => FireBall,
                SID.LightingVolt => LighteningVolt,
                SID.KnightSpecial => KnightSpecial,
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}
