using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;

namespace KWY
{
    public class ErrorMsg : MonoBehaviour
    {
        static GameObject _errorMsg;

        static GameObject errorMsg
        {
            get { return _errorMsg; }
        }

        static ErrorMsg _instance;

        public static ErrorMsg Instance
        {
            get
            {
                if (!_instance)
                {
                    _errorMsg = new GameObject
                    {
                        name = "ErrorMsg"
                    };
                    _instance = errorMsg.AddComponent(typeof(ErrorMsg)) as ErrorMsg;
                    DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }

       // public string ErrorMsgFileName = "Error_Msg";

        private ErrorMsgData _eData;
        public ErrorMsgData eData
        {
            get
            {
                if (_eData == null)
                {
                    LoadErrorMsg();
                }
                return _eData;
            }
        }

        public void LoadErrorMsg()
        {
            // Error_Msg를 변수로 넣어주면 파일을 못 찾음 왜?
            TextAsset jsonText = Resources.Load("Error_Msg") as TextAsset;

            if (jsonText == null)
            {
                Debug.Log("There is no error msg file. Check the file.");

                // error

                return;
            }
            else
            {
                _eData = JsonUtility.FromJson<ErrorMsgData>(jsonText.text);
                Debug.Log("Error msg file is loaded");

                Debug.Log(_eData);
            }
        }

        private void Start()
        {
            LoadErrorMsg();
        }
    }
}

