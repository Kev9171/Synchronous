using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace KWY
{
    class SpawnableSkillResources
    {
        public static string FireBall = "Prefabs/SpawnableSkills/FireBall";
        public static string KnightNormal = "Prefabs/SpawnableSkills/KnightNormal";
        public static string KnightSpecial = "Prefabs/SpawnableSkills/KnightSpecial";
        public static string LighteningVolt = "Prefabs/SpawnableSkills/LightningVolt";
        public static string Heal = "Prefabs/SpawnableSkills/Heal";
        public static string SpearAttack = "Prefabs/SpawnableSkills/SpearAttack";

        public static string GetPath(SID sid)
        {
            return sid switch
            {
                SID.FireBall => FireBall,
                SID.LightingVolt => LighteningVolt,
                SID.KnightNormal => KnightNormal,
                SID.KnightSpecial => KnightSpecial,
                SID.Heal => Heal,
                SID.SpearAttack => SpearAttack,
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}
