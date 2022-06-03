using UnityEngine;
using System.Collections.Generic;

namespace KWY
{
    /// <summary>
    /// 스킬에 대한 기본 정보가 담겨 있는 클래스 (최초 스킬 정보 로드시 사용)
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
