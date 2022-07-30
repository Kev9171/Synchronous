using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    [CreateAssetMenu(fileName = "CharaManager", menuName = "Singletons/CharacterManager")]
    public class CharaManager : SingletonScriptableObject<CharaManager>
    {
        [SerializeField]
        private List<CharacterBase> _characters = new List<CharacterBase>();


        private Dictionary<CID, CharacterBase> _characterData = new Dictionary<CID, CharacterBase>();

        public static Dictionary<CID, CharacterBase> CharacterData { get { return Instance._characterData; } }


        private static bool loaded = false;
        public static CharacterBase GetData(CID cid)
        {
            if (!loaded)
            {
                FirstInitialize();
            }
            if (CharacterData.TryGetValue(cid, out var value))
            {
                //Debug.Log("Found value on " + cid + ": " + value);
                return value;
            }
            else
            {
                Debug.LogErrorFormat("Can not find : {0}, add the object at manager.", cid);
                return null;
            }
        }

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void FirstInitialize()
        {
            //Debug.Log("This message will output before Awake");

            // make dictionary with key: SID
            foreach (var cb in Instance._characters)
            {
                Instance._characterData.Add(cb.cid, cb);
            }
            

            loaded = true;
        }
    }
}
