using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    [CreateAssetMenu(fileName = "MoveManger", menuName = "Singletons/MoveManager")]
    public class MoveManager : SingletonScriptableObject<MoveManager>
    {
        [SerializeField]
        private MoveBase _move;

        public static MoveBase MoveData { get { return Instance._move; } }
    }
}
