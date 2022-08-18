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

        // ���� � �ȵǴϱ� �� Ž���ص� ������ �Ȱ���...?
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
