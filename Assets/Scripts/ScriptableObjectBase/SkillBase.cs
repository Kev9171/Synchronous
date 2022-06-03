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
        public List<Vector2Int> area = new List<Vector2Int>();

        public override string ToString()
        {
            return string.Format("SID: {0}, SkillName: {1}, Cost: {2}", sid, name, cost);
        }
    }
}
