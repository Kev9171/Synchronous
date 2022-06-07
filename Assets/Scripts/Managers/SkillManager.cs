using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    /// <summary>
    /// ��� ��ų scriptable object�� �����ϴ� singleton object
    /// </summary>
    [CreateAssetMenu(fileName = "SkillManager", menuName = "Singletons/SkillManager")]
    public class SkillManager : SingletonScriptableObject<SkillManager>
    {
        [SerializeField]
        private List<SkillBase> _skills = new List<SkillBase>();

        private Dictionary<SID, SkillBase> _skillData = new Dictionary<SID, SkillBase>();

        public static Dictionary<SID, SkillBase> SkillData { get { return Instance._skillData; } }

        private static bool loaded = false;

        public static SkillBase GetData(SID sid)
        {
            if (!loaded) FirstInitialize();

            if (SkillData.TryGetValue(sid, out var value))
            {
                Debug.Log("Found value on " + sid + ": " + value);
                return value; 
            }
            else
            {
                Debug.LogErrorFormat("Can not find : {0}, add the object at manager.", sid);
                return null;
            }
        }

        private static void FirstInitialize()
        {
            // make dictionary with key: SID
            foreach (var sb in Instance._skills)
            {
                Instance._skillData.Add(sb.sid, sb);
            }

            loaded = true;

        }
    }
}
