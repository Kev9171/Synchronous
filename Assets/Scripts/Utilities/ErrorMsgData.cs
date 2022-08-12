using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace KWY
{
    [Serializable]
    public class ErrorMsgData
    {
        public List<ErrorMsgStruct> data;

        // 에러 몇개 안되니까 다 탐색해도 느리진 안겠죠...?
        public string getMsg(ErrorCode code)
        {
            foreach(var s in data)
            {
                if (s.error_code == (int) code)
                {
                    return s.error_msg;
                }
            }
            Debug.LogError($"There is no such error code: {code}");
            return null;
        }

        public override string ToString()
        {
            string s = "";

            foreach(var v in data)
            {
                s += v + ", ";
            }
            return s;
        }
    }
}
