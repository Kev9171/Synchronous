using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using KWY;
using System.IO;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance;

    private string currentScene;

    new AudioSource audio;

    private struct MUSIC
    {
        public const string MyFirstGame = "Music/MyFirstGame";
        public const string TheVideoGame = "Music/TheVideoGame";
        public const string WarNerveLoop = "Music/WarNerveLoop";
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        audio = GetComponent<AudioSource>();

        if (SettingManager.Instance != null)
        {
           Debug.Log("The setting file is loaded.");
        }
    }

    public void PlayMusicSceneChanged(Scene previousScene, Scene newActiveScene)
    {
        // �� �ٲ� previousScene.name�� null ��
        string prev = previousScene.name;
        string now = newActiveScene.name;

        if (prev == null)
        {
            prev = currentScene;
        }

        if (prev.Equals("StartScene") && now.Equals("GameLobby") || prev.Equals("GameLobby") && now.Equals("StartScene"))
        {
            ;
        }
        else
        {
            PlayMusic();
        }
        currentScene = now;
    }

    public void PlayMusic()
    {
        StopMusic();

        switch(SceneManager.GetActiveScene().name)
        {
            case "StartScene":
            case "GameLobby":
                audio.clip = Resources.Load<AudioClip>(MUSIC.TheVideoGame);
                if (audio.clip == null)
                {
                    Debug.LogWarning($"Can not find audio sources - {MUSIC.TheVideoGame}");
                    return;
                }
                break;
            case "PickScene":
                audio.clip = Resources.Load<AudioClip>(MUSIC.MyFirstGame);
                if (audio.clip == null)
                {
                    Debug.LogWarning($"Can not find audio sources - {MUSIC.MyFirstGame}");
                    return;
                }
                break;
            case "MainGameScene":
                audio.clip = Resources.Load<AudioClip>(MUSIC.WarNerveLoop);
                if (audio.clip == null)
                {
                    Debug.LogWarning($"Can not find audio sources - {MUSIC.WarNerveLoop}");
                    return;
                }
                break;
            default:
                return;
        }

        audio.Play();
    }

    public void PauseMusic()
    {
        audio.Pause();
    }

    public void StopMusic()
    {
        audio.Stop();
    }

    public void ApplySettings(GamePlaySettingData data)
    {
        // ���⼭ �� ����
        audio.volume = data.BGM_Volume;

        // mute ���� ������ ���⼭
        audio.mute = data.BGM_Mute;
    }

    public void ApplySettings()
    {
        string filePath = Application.persistentDataPath + SettingManager.Instance.SettingFileName;

        if (File.Exists(filePath))
        {
            Debug.Log("Game Setting File is loaded.");
            string _data = File.ReadAllText(filePath);
            GamePlaySettingData data = JsonUtility.FromJson<GamePlaySettingData>(_data);

            if (data != null)
            {
                // ���⼭ �� ����
                audio.volume = data.BGM_Volume;

                // mute ���� ������ ���⼭
                audio.mute = data.BGM_Mute;
            }
        }
        else
        {
            Debug.LogError("Can not find setting file.");
        }
    }

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        SceneManager.activeSceneChanged += PlayMusicSceneChanged;
    }

    private void Update()
    {
        
    }
}
