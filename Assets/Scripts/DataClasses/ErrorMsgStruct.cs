using System;

namespace KWY
{
    [Serializable]
    public class ErrorMsgStruct
    {
        public int error_code;
        public string error_msg;

        public override string ToString()
        {
            return string.Format("[ErrorCode: {0}, ErrorMsg: {1}]", error_code, error_msg);
        }
    }
}
