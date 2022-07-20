using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneUI : MonoBehaviour
{
    public Button LoginBtn;
    public Button GameStartBtn;
    public Button JoinBtn;


    //public GameObject JoinButton;

    public void LoginClicked()
    {
        Debug.Log("Login Clicked!");
    }
    
    public void SignUpClicked()
    {
        Debug.Log("SignUp Clicked!");
    }
    
    public void CreateClicked()
    {
        Debug.Log("Create Clicked!");
        SceneManager.LoadScene("Rooms");
    }

    public void JoinClicked()
    {
        Debug.Log("Join Clicked!");
    }
}
