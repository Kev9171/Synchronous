using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public GameObject LoginButton;
    public GameObject SignUpButton;
    public GameObject CreateButton;
    public GameObject JoinButton;

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
    }

    public void JoinClicked()
    {
        Debug.Log("Join Clicked!");
    }

    void Awake()
    {
        
    }

}
