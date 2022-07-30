using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    [CreateAssetMenu(fileName = "BuffManager", menuName = "Singletons/BuffManager")]
    public class BuffManager : SingletonScriptableObject<BuffManager>
    {
        [SerializeField]
        private List<BuffBase> _buffs = new List<BuffBase>();

        private Dictionary<BID, BuffBase> _buffData = new Dictionary<BID, BuffBase>();

        private static bool loaded = false;
        public static Dictionary<BID, BuffBase> BuffData { get { return Instance._buffData; } }
        public static BuffBase GetData(BID bid)
        {
            if (!loaded) FirstInitialize();
            if (BuffData.TryGetValue(bid, out var value))
            {
                //Debug.Log("Found value on " + bid + ": " + value);
                return value;
            }
            else
            {
                Debug.LogErrorFormat("Can not find : {0}, add the object at manager.", bid);
                return null;
            }
        }

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void FirstInitialize()
        {
            //Debug.Log("This message will output before Awake");

            // make dictionary with key: BID
            foreach (var bb in Instance._buffs)
            {
                Instance._buffData.Add(bb.bid, bb);
            }
            loaded = true;
        }
    }
}
