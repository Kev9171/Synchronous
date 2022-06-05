using Photon.Pun;
using Photon.Realtime;

using System.Collections.Generic;

using ExitGames.Client.Photon;

using UnityEngine;

namespace KWY 
{
    public static class UtilForDebug
    {
        public static void LogData(EventData eventData)
        {
            byte code = eventData.Code;
            object[] data = (object[])eventData.CustomData;

            string content = "";

            if (!(data is object[]))
            {
                return;
            }
            foreach (object a in data)
            {
                content += (string)a + ", ";
            }

            Debug.LogFormat("EventData: {0}, [{1}]", code, content);
        }

        public static void LogRaiseEvent(byte evcode, object content, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            string str = "";

            if (content is object[])
            {
                foreach (object a in (object[])content)
                {
                    content += a.ToString() + ", ";
                }
            }

            // action data
            else if (content is Dictionary<int, int>)
            {
                Dictionary<int, int> data = (Dictionary<int, int>)content;
                foreach (int i in data.Keys)
                {
                    data.TryGetValue(i, out int value);
                    content += (i + ": " + value + ", ");
                }
            }

            Debug.LogFormat("evcode: {0}, data: {1}, RaiseEventOptions: {2}, SendOptions: {3}", evcode, str, raiseEventOptions.ToString(), sendOptions.ToString());
        }

        public static void LogErrorRaiseEvent(byte evCode)
        {
            Debug.LogErrorFormat("Failed to send Event: {0} to the server", evCode);
        }
    }
}



