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

        public override string ToString()
        {
            return string.Format("bid: {0}, buffName: {1}", bid, buffName);
        }
    }
}
