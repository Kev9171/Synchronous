using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


namespace KWY {
    public class Event_Sample : MonoBehaviour
    {
        #region Private Fields

        #endregion 

        void Awake()
        {
        }

        [PunRPC]
        public void SendTest(string msg) {
            Debug.Log(string.Format("Msg: {0}", msg));
        }


        public void RaiseEventTest(string msg) {
            Debug.Log("FunCalled - RaiseEventTest");
            Debug.Log("msg: " + msg);

            byte evCode = 100;
            object[] content = new object[] {
                msg, 0, 1, new Vector2(10, 20), false
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions{
                Receivers = ReceiverGroup.All
            };

            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);

            Debug.Log(string.Format("RaiseEventTest: msg {0}", msg));
        }

        public void RaiseEventTestForVector2Int(Vector2Int v)
        {
            Debug.Log("FunCalled - RaiseEventTestForVector2");
            Debug.LogFormat("Content: {0}", v);

            byte evCode = 101;
            object[] content = new object[] {
                v.x, v.y
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions{
                Receivers = ReceiverGroup.All
            };

            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);

            Debug.Log(string.Format("RaiseEventTestForVector2Int: {0}", v));
        }
    }
}

