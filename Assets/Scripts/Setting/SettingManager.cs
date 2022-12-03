using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace KWY
{
    /// <summary>
    /// Singleton - Scene에 추가할 필요 없이 static 객체로 Instance 변수를 가져와서 쓰면 됨
    /// ex) SettingManager.Instance.~~~~
    /// </summary>
    public class SettingManager : MonoBehaviour
    {
        static GameObject _settings;
        static GameObject settings
        {
            get { return _settings; }
        }
        static SettingManager _instance;
        public static SettingManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _settings = new GameObject
                    {
                        name = "Settings"
                    };
                    _instance = settings.AddComponent(typeof(SettingManager)) as SettingManager;
                    DontDestroyOnLoad(_settings);
                }
                return _instance;
            }
        }

        public readonly string SettingFileName = "game_settings.json";

        private GamePlaySettingData _gameSettings;
        public GamePlaySettingData gameSettings
        {
            get
            {
                if (_gameSettings == null)
                {
                    LoadGameSettings();
                    SaveGameSettings();
                }
                return _gameSettings;
            }
        }

        public void LoadGameSettings()
        {
            string filePath = Application.persistentDataPath + SettingFileName;

            if (File.Exists(filePath))
            {
                Debug.Log("Game Setting File is loaded.");
                string data = File.ReadAllText(filePath);
                _gameSettings = JsonUtility.FromJson<GamePlaySettingData>(data);
            }
            else
            {
                Debug.Log("There is no game-play-setting file. Creating a new file...");
                _gameSettings = new GamePlaySettingData();
            }
        }

        public void SaveGameSettings()
        {
            string data = JsonUtility.ToJson(gameSettings);
            string filePath = Application.persistentDataPath + SettingFileName;

            // 이미 저장된 파일 있을 경우 덮어쓰기
            File.WriteAllText(filePath, data);

            // for test
            Debug.Log(data);

            // 데이터 적용
            if (BackgroundMusic.Instance != null)
            {
                BackgroundMusic.Instance.ApplySettings();
                Debug.Log("The setting values are applied");
            }
        }

        /*// 게임 종료시 실행 되는 Callback Function
        private void OnApplicationQuit()
        {
            // 설정 저장
            SaveGameSettings();
        }*/

        private void Start()
        {
            LoadGameSettings();
            SaveGameSettings();
        }

    }
}
