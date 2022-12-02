using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "PickScene")
        {
            BackgroundMusic.instance.GetComponent<AudioSource>().Pause();
        }

        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            BackgroundMusic.instance.GetComponent<AudioSource>().Play();

        }
    }
}
