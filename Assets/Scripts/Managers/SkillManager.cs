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
        private List<SkillBase> _skills = new List< SkillBase>();

        private Dictionary<SID, SkillBase> _skillData = new Dictionary<SID, SkillBase>();

        public static Dictionary<SID, SkillBase> SkillData { get { return Instance._skillData; } }

        public static SkillBase GetData(SID sid)
        {
            if (SkillData.TryGetValue(sid, out var value))
            {
                Debug.Log("Found value on " + sid + ": " + value);
                Debug.Log("Hash: " + value.GetHashCode());
                return value; 
            }
            else
            {
                return null;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void FirstInitialize()
        {
            //Debug.Log("This message will output before Awake");

            // make dictionary with key: SID
            foreach (var sb in Instance._skills)
            {
                Instance._skillData.Add(sb.sid, sb);
            }

            //Debug.Log("�ּҰ� �� - �������� ��������"); // ok. reference ��
            //Debug.Log("Instance skills[]:" + Instance._skills[0].GetHashCode());
            //Debug.Log("Instance skilldata[]:" + Instance._skillData[(SID)1].GetHashCode());
        }
    }
}
