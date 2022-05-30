using Photon.Pun;

using ExitGames.Client.Photon;

using UnityEngine;
using UnityEngine.UI;


namespace KWY {
    class EventCallBack_Sample : MonoBehaviourPun {

        public Text logText;
        public GameObject chara;

        public void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        public void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }


        /// <summary>
        /// evCode=100: data[0]:string
        /// evCode=101: data[0]:Vector2Int, data[1]:x:int, data[2]:y:int
        /// </summary>
        /// <param name="photonEvent">received event data from the Server</param>
        public void OnEvent(EventData photonEvent) {
            byte evCode = photonEvent.Code;

            Debug.Log("FunCalled - OnEvent");
            Debug.Log("EventData: " + photonEvent);
            Debug.Log("evCode: " + evCode);

            if (evCode == 100) 
            {
                object[] data = (object[])photonEvent.CustomData;
                if ((string)data[0] == "Error")
                {
                    Debug.LogFormat("OnEvent: received data is '{0}'", (string)data[0]);
                    return;
                }
                Debug.Log("OnEvent: " + data[0]);

                logText.text += "\n" + data[0].ToString();
            }

            else if (evCode == 101) 
            {
                object[] data = (object[])photonEvent.CustomData;

                if ((string)data[0] == "Error")
                {
                    Debug.LogFormat("OnEvent: received data is '{0}'", (string)data[0]);
                    return;
                }

                Debug.LogFormat("OnEvent: " + data[1] + ", " + data[2]);
                logText.text += "\nOnEvent: " + data[1] + ", " + data[2];
                chara.GetComponent<ICharacter>().MoveTo((int)data[1], (int)data[2]);
            }
        }
    }
}
