using UnityEngine;
using System.Collections.Generic;

namespace KWY
{
    /// <summary>
    /// ��ų�� ���� �⺻ ������ ��� �ִ� Ŭ���� (���� ��ų ���� �ε�� ���)
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterSkill", menuName = "CharacterSkill/SkillBase")]
    public class SkillBase : ActionBase
    {
        public SID sid;
        public List<CID> casters = new List<CID>();
        public string skillExplanation;
        public Sprite skillExImg;
        public List<float> distance = new List<float>();
        public List<Direction> directions = new List<Direction>();
        public bool areaAttack;
<<<<<<< Updated upstream:Assets/Scripts/ScriptableObjectBase/SkillBase.cs
        public skillSpawner area;
=======
        public SkillSpawner area;
        public bool isDamage;
        public int value; // ������ �Ǵ� ������ ��ġ
>>>>>>> Stashed changes:Assets/Scripts/Scriptable Object/ScriptableObjectBase/SkillBase.cs

        public override string ToString()
        {
            return string.Format("SID: {0}, SkillName: {1}, CastingTime: {2}, Cost: {3}", sid, name, castingTime, cost);
        }
    }
}
