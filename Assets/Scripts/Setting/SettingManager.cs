using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace KWY
{
    /// <summary>
    /// Singleton - Scene�� �߰��� �ʿ� ���� static ��ü�� Instance ������ �����ͼ� ���� ��
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

            // �̹� ����� ���� ���� ��� �����
            File.WriteAllText(filePath, data);

            // for test
            Debug.Log(data);

            // ������ ����
            if (BackgroundMusic.Instance != null)
            {
                BackgroundMusic.Instance.ApplySettings();
                Debug.Log("The setting values are applied");
            }
        }

        /*// ���� ����� ���� �Ǵ� Callback Function
        private void OnApplicationQuit()
        {
            // ���� ����
            SaveGameSettings();
        }*/

        private void Start()
        {
            LoadGameSettings();
            SaveGameSettings();
        }

    }
}
