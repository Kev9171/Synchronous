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
        public List<CID> casters = new List<CID>();
        public string skillExplanation;
        public Sprite skillExImg;
        public List<float> distance = new List<float>();
        public List<Direction> directions = new List<Direction>();

        public override string ToString()
        {
            return string.Format("SID: {0}, SkillName: {1}, CastingTime: {2}, Cost: {3}", sid, name, castingTime, cost);
        }
    }
}
