using UnityEngine.UI;
using UnityEngine;
using System;

namespace KWY
{
    public class CharacterTestSceneUI : MonoBehaviour
    {
        public InputField InputMsg;
        public InputField InputX;
        public InputField InputY;

        public Event_Sample e;

        private int vx = Int32.MinValue, vy = Int32.MinValue;

        public void SendMsg()
        {
            if (!string.IsNullOrEmpty(InputMsg.text))
            {
                 e.RaiseEventTest(InputMsg.text);
            }
            else
            {
                Debug.LogError("The message is null or empty");
            }
            
        }

        public void SendVector2Int()
        {
            SetX(InputX.text);
            SetY(InputY.text);

            if (vx != Int32.MinValue && vy != Int32.MinValue)
            {
                e.RaiseEventTestForVector2Int(new Vector2Int(vx, vy));
            }
        }

        public void SetX(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Coords X is null or empty");
                return;
            }
            
            if (!Int32.TryParse(value, out vx))
            {
                Debug.LogError("X should be an integer");
            }
        }

        public void SetY(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Coords Y is null or empty");
                return;
            }
            
            if (!Int32.TryParse(value, out vy))
            {
                Debug.LogError("Y should be an integer");
            }
        }
    }
}