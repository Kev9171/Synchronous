using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    [CreateAssetMenu(fileName = "PlayerSkillManager", menuName = "Singletons/PlayerSkillManager")]
    public class PlayerSkillManager : SingletonScriptableObject<PlayerSkillManager>
    {
        [SerializeField]
        private List<PlayerSkillBase> _skills = new List<PlayerSkillBase>();

        private Dictionary<PSID, PlayerSkillBase> _skillData = new Dictionary<PSID, PlayerSkillBase>();

        public static Dictionary<PSID, PlayerSkillBase> PSkillData { get { return Instance._skillData; } }

        private static bool loaded = false;

        public static PlayerSkillBase GetData(PSID psid)
        {
            if(!loaded)
            {
                FirstInitialize();
            }

            if (PSkillData.TryGetValue(psid, out var value))
            {
                //Debug.Log("Found value on " + psid + ": " + value);
                return value;
            }
            Debug.LogErrorFormat("Can not find : {0}, add the object at manager.", psid);
            return null;
        }

        private static void FirstInitialize()
        {
            foreach(var psb in Instance._skills)
            {
                Instance._skillData.Add(psb.psid, psb);
            }
            loaded = true;
        }
    }
}
