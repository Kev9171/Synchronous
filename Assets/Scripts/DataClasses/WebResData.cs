using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KWY
{
    [Serializable]
    public class LoginResData
    {
        public int code;
        public string message;
        public ulong uid;
        public string id;
        public int level;
        public string name;
        public string imageUrl;

        // for test
        public LoginResData(int a, string b, ulong c, string d, int e, string f, string g)
        {
            code = a;
            message = b;
            uid = c;
            id = d;
            level = e;
            name = f;
            imageUrl = g;
        }
    }

    [Serializable]
    public class NameCheckResData
    {
        public int code;
        public string message;

        // for test
        public NameCheckResData(int a, string b)
        {
            code = a;
            message = b;
        }
    }

    [Serializable]
    public class IdCheckResData
    {
        public int code;
        public string message;

        // for test
        public IdCheckResData(int a, string b)
        {
            code = a;
            message = b;
        }
    }

    [Serializable]
    public class JoinResData
    {
        public int code;
        public string message;
        public ulong uid;

        public JoinResData(int a, string b, ulong c)
        {
            code = a;
            message = b;
            uid = c;
        }
    }

    public enum ResCode : int
    {
        TRUE = 1,
        ERROR = -1,
        FALSE = 0,
    }
}
