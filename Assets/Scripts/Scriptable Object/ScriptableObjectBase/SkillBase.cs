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
        public List<float> distance = new List<float>(); // area skill에서 distance[0]은 범위를 듯하고 distance[1]은 area skill의 사거리를 뜻함
        public List<Direction> directions = new List<Direction>();
        public bool areaAttack;
        public SkillSpawner area;
        public bool isDamage;
        public int value; // 데미지 또는 버프의 수치

        public float dmgMultiplicand;

        public override string ToString()
        {
            return string.Format("SID: {0}, SkillName: {1}, CastingTime: {2}, Cost: {3}", sid, name, castingTime, cost);
        }
    }
}
