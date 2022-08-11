using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace KWY
{
    public class LoginJoinAPI
    {
        /*static GameObject _webAPI;

        static GameObject WebAMI
        {
            get { return _webAPI; }
        }*/

        // for Singleton
        static LoginJoinAPI _instance;
        public static LoginJoinAPI Instance
        {
            get
            {
                if (_instance == null)
                {
                    

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

        private void LoadAPIUrl()
        {
            // load json first
            TextAsset jsonText = Resources.Load("WebLoginAPI") as TextAsset;

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


        public IEnumerator LoginPost(string id, string pw, UnityAction<bool> callback)
        {
            string url = Urls.ip_url + "/" + Urls.login_url;

            WWWForm form = new WWWForm();

            form.AddField("id", id);
            form.AddField("password", pw);

            UnityWebRequest uwr = UnityWebRequest.Post(url, form);

            yield return uwr.SendWebRequest();

            if (uwr.error == null)
            {
                string result = uwr.downloadHandler.text;
                Debug.Log(result);

                LoginResData data = JsonUtility.FromJson<LoginResData>(result);

                if (data.code != (int)ResCode.GOOD)
                {
                    // error
                }

                callback(data.message);
            }
            // error occured
            else
            {
                // request error
            }
        }

        public IEnumerator IdCheckPost(string id, UnityAction<bool> callback)
        {
            string url = Urls.ip_url + "/" + Urls.id_check_url;

            WWWForm form = new WWWForm();

            form.AddField("id", id);

            UnityWebRequest uwr = UnityWebRequest.Post(url, form);

            yield return uwr.SendWebRequest();

            if (uwr.error == null)
            {
                string result = uwr.downloadHandler.text;
                Debug.Log(result);

                IdCheckResData data = JsonUtility.FromJson<IdCheckResData>(result);

                if (data.code != (int)ResCode.GOOD)
                {
                    // error
                }

                callback(data.message);
            }
            // error occured
            else
            {

            }
        }

        public IEnumerator NameCheckPost(string name, UnityAction<bool> callback)
        {
            string url = Urls.ip_url + "/" + Urls.name_check_url;

            WWWForm form = new WWWForm();

            form.AddField("name", name);

            UnityWebRequest uwr = UnityWebRequest.Post(url, form);

            yield return uwr.SendWebRequest();

            if (uwr.error == null)
            {
                string result = uwr.downloadHandler.text;
                Debug.Log(result);

                NameCheckResData data = JsonUtility.FromJson<NameCheckResData>(result);

                if (data.code != (int)ResCode.GOOD)
                {
                    // error
                }

                callback(data.message);
            }
            // error occured
            else
            {

            }
        }

        public IEnumerator JoinPost(string id, string name, string pw, UnityAction<bool, string> callback)
        {
            string url = Urls.ip_url + "/" + Urls.join_url;

            WWWForm form = new WWWForm();

            form.AddField("name", name);

            UnityWebRequest uwr = UnityWebRequest.Post(url, form);

            yield return uwr.SendWebRequest();

            if (uwr.error == null)
            {
                string result = uwr.downloadHandler.text;
                Debug.Log(result);

                JoinResData data = JsonUtility.FromJson<JoinResData>(result);

                if (data.code != (int)ResCode.GOOD)
                {
                    // error
                }

                // if message is fales, uid is null
                callback(data.message, data.uid);
            }
            // error occured
            else
            {

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
