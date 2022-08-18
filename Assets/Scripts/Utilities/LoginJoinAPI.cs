using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace KWY
{
    public class LoginJoinAPI : MonoBehaviour, ILoginJoinAPI
    {
        static GameObject _webAPI;

        static GameObject WebAMI
        {
            get { return _webAPI; }
        }

        // for Singleton
        static LoginJoinAPI _instance;
        public static LoginJoinAPI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _webAPI = new GameObject
                    {
                        name = "WebAPI"
                    };
                    _instance = _webAPI.AddComponent(typeof(LoginJoinAPI)) as LoginJoinAPI;
                    DontDestroyOnLoad(_webAPI);
                }
                return _instance;
            }
        }

        private APIUrl _urls;
        public APIUrl Urls
        {
            get
            {
                if (_urls == null)
                {
                    LoadAPIUrl();
                }
                return _urls;
            }
        }

        const int timeout_seconds = 5;

        private void LoadAPIUrl()
        {
            // load json first
            //TextAsset jsonText = Resources.Load("WebLoginAPI") as TextAsset;
            TextAsset jsonText = Resources.Load("WebLoginAPI copy") as TextAsset;

            if (jsonText == null)
            {
                Debug.LogError("There is no api file. Check the file");
                return;
            }
            else
            {
                _urls = JsonUtility.FromJson<APIUrl>(jsonText.text);
                Debug.Log("APIUrl file is loaded");
            }
        }


        public IEnumerator LoginPost(string id, string pw, UnityAction<LoginResData> callback, UnityAction<ErrorCode> errorCallback)
        {
            string url = Urls.ip_url + "/" + Urls.login_url;

            WWWForm form = new WWWForm();

            form.AddField("id", id);
            form.AddField("password", pw);

            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            uwr.timeout = timeout_seconds;

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string result = uwr.downloadHandler.text;

                LoginResData data = JsonUtility.FromJson<LoginResData>(result);

                callback(data);
            }
            else if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                errorCallback(ErrorCode.WEB_REQUEST_CONNECTION_ERROR);
            }
            else if (uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                errorCallback(ErrorCode.WEB_REQUEST_PROTOCOL_ERROR);
            }
            else
            {
                errorCallback(ErrorCode.WEB_REQUEST_ERROR);
            }            
        }

        public IEnumerator IdCheckPost(string id, UnityAction<IdCheckResData> callback, UnityAction<ErrorCode> errorCallback)
        {
            string url = Urls.ip_url + "/" + Urls.id_check_url;

            WWWForm form = new WWWForm();

            form.AddField("id", id);

            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            uwr.timeout = timeout_seconds;

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string result = uwr.downloadHandler.text;
                Debug.Log(result);

                IdCheckResData data = JsonUtility.FromJson<IdCheckResData>(result);

                callback(data);
            }
            else if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                errorCallback(ErrorCode.WEB_REQUEST_CONNECTION_ERROR);
            }
            else if (uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                errorCallback(ErrorCode.WEB_REQUEST_PROTOCOL_ERROR);
            }
            else
            {
                errorCallback(ErrorCode.WEB_REQUEST_ERROR);
            }
        }

        public IEnumerator NameCheckPost(string name, UnityAction<NameCheckResData> callback, UnityAction<ErrorCode> errorCallback)
        {
            string url = Urls.ip_url + "/" + Urls.name_check_url;

            WWWForm form = new WWWForm();

            form.AddField("name", name);

            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            uwr.timeout = timeout_seconds;

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string result = uwr.downloadHandler.text;
                Debug.Log(result);

                NameCheckResData data = JsonUtility.FromJson<NameCheckResData>(result);

                callback(data);
            }
            else if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                errorCallback(ErrorCode.WEB_REQUEST_CONNECTION_ERROR);
            }
            else if (uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                errorCallback(ErrorCode.WEB_REQUEST_PROTOCOL_ERROR);
            }
            else
            {
                errorCallback(ErrorCode.WEB_REQUEST_ERROR);
            }
        }

        public IEnumerator JoinPost(string id, string name, string pw, UnityAction<JoinResData> callback, UnityAction<ErrorCode> errorCallback)
        {
            string url = Urls.ip_url + "/" + Urls.join_url;

            WWWForm form = new WWWForm();

            form.AddField("id", id);
            form.AddField("name", name);
            form.AddField("password", pw);

            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            uwr.timeout = timeout_seconds;

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string result = uwr.downloadHandler.text;
                Debug.Log(result);

                JoinResData data = JsonUtility.FromJson<JoinResData>(result);

                callback(data);
            }
            else if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                errorCallback(ErrorCode.WEB_REQUEST_CONNECTION_ERROR);
            }
            else if (uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                errorCallback(ErrorCode.WEB_REQUEST_PROTOCOL_ERROR);
            }
            else
            {
                errorCallback(ErrorCode.WEB_REQUEST_ERROR);
            }
        }
    }

    public class APIUrl
    {
        public string ip_url;
        public string login_url;
        public string join_url;
        public string id_check_url;
        public string name_check_url;
        public string logout_url;
    }
}
