using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    [CreateAssetMenu(fileName = "OLogicData", menuName = "OLogicData")]
    public class SLogicData : ScriptableObject
    {
        public int playerInitialMp;
        public int playerMPIncrement;
        public int characterMPIncrement;
        public float timeLimit;

        public float actionLogShowingTime;

        public float simulationIntervalSeconds;


        public int characterScoreMultiplier;
        public int playerSkillCountScoreMultiplier;
    }
}
