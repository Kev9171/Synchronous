using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class LogicData : MonoBehaviour
    {
        static GameObject _logicData;
        static GameObject logicData
        {
            get { return _logicData; }
        }

        static LogicData _instance;
        public static LogicData Instance
        {
            get
            {
                if (!_instance)
                {
                    _logicData = new GameObject
                    {
                        name = "LogicData"
                    };
                    _instance = logicData.AddComponent(typeof(LogicData)) as LogicData;
                    DontDestroyOnLoad(_logicData);
                }
                return _instance;
            }
        }

        private int playerInitialMp;
        private int playerMPIncrement;
        private int characterMPIncrement;
        private float timeLimit;
        private float actionLogShowingTime;
        private float simulationIntervalSeconds;

        private int characterScoreMultiplier;
        private int playerSkillCountScoreMultiplier;

        private bool loaded = false;
        private bool loadFailed = false;

        public int PlayerInitialMp { 
            get 
            { 
                if (!loaded)
                {
                    LoadValues();
                }
                return playerInitialMp;
            } 
        }
        public int PlayerMPIncrement { get { if (!loaded) { LoadValues(); } return playerMPIncrement; } }
        public int CharacterMpIncrement { get { if (!loaded) { LoadValues(); } return characterMPIncrement; } }
        public float TimeLimit { get { if (!loaded) { LoadValues(); } return timeLimit; } }
        public float ActionLogShowingTime { get { if (!loaded) { LoadValues(); } return actionLogShowingTime; } }
        public float SimulationIntervalSeconds { get { if (!loaded) { LoadValues(); } return simulationIntervalSeconds; } }

        public int CharacterScoreMultiplier { get { if (!loaded) { LoadValues(); } return characterScoreMultiplier; } }
        public int PlayerSkillCountScoreMultiplier { get { if (!loaded) { LoadValues(); } return playerSkillCountScoreMultiplier; } }
        public void LoadValues()
        {
            if (loadFailed)
            {
                return;
            }

            SLogicData data = Resources.Load<SLogicData>("OLogicData");

            if (data != null)
            {
                playerInitialMp = data.playerInitialMp;
                playerMPIncrement = data.playerMPIncrement;
                characterMPIncrement = data.characterMPIncrement;
                timeLimit = data.timeLimit;
                actionLogShowingTime = data.actionLogShowingTime;
                simulationIntervalSeconds = data.simulationIntervalSeconds;
                characterScoreMultiplier = data.characterScoreMultiplier;
                playerSkillCountScoreMultiplier = data.playerSkillCountScoreMultiplier;

                loaded = true;
            }
            else
            {
                Debug.LogError("OLogicData can not be found");
                loadFailed = true;
            }
            
        }

        private void Start()
        {
            if (!loaded)
            {
                LoadValues();
            }
        }
    }
}
