using UnityEngine;

namespace KWY
{
    [CreateAssetMenu(fileName = "PlayerSkillBase", menuName ="PlayerSkill/PlayerSkillBase")]
    public class PlayerSkillBase : ScriptableObject
    {
        public PSID psid;
        public new string name;
        public string skillExplanation;
        public Sprite icon;
        public int cost;

        public override string ToString()
        {
            return string.Format("PSID: {0}, Name: {1}, Cost : {2}", psid, name, cost);
        }
    }
}
