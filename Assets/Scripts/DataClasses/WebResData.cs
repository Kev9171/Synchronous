using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class LoginResData
    {
        public int code;
        public bool message;
    }

    public class NameCheckResData
    {
        public int code;
        public bool message;
    }

    public class IdCheckResData
    {
        public int code;
        public bool message;
    }

    public class JoinResData
    {
        public int code;
        public bool message;
        public string uid;
    }

    public enum ResCode : int
    {
        GOOD = 0,
        BAD = -1
    }
}
