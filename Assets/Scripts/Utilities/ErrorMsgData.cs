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

        // 단순히 탐색 (에러 종료 몇 안되니까 전부 탐색...?)
        public string[] GetData(ErrorCode e)
        {
            foreach(ErrorMsgStruct s in data)
            {
                if (s.ErrorNum == (int)e)
                {
                    return new string[] { s.ErrorCode, s.ErrorContent };
                }
            }
            return null;
        }

        public override string ToString()
        {
            string s = "";
            foreach(ErrorMsgStruct e in data)
            {
                s += e + ", ";
            }

            return s;
        }
    }

    [Serializable]
    public class ErrorMsgStruct
    {
        public int ErrorNum;
        public string ErrorCode;
        public string ErrorContent;

        public override string ToString()
        {
            return string.Format("[ErroNum: {0}, ErrorCode: {1}, ErrorContent: {2}]", ErrorNum, ErrorCode, ErrorContent);
        }
    }
}
