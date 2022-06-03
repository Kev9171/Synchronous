using UnityEngine;

namespace KWY
{
    [CreateAssetMenu(fileName = "Buff", menuName = "Buff/BuffBase")]
    public class BuffBase : ScriptableObject
    {
        public BID bid;
        public string buffName;
        public Sprite icon;
        public string explanation;
    }
}
