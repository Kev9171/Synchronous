using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    [CreateAssetMenu(fileName = "MainGameLogicData", menuName = "MainGameLogicData")]
    public class LogicData : ScriptableObject
    {
        public int playerMPIncrement;
        public int characterMPIncrement;
        public float timeLimit;

        public float actionLogShowingTime;
    }
}
