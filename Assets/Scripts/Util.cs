#define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class Util
    {
        const string st = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GetRandomString(int len = -1)
        {
            if (len == -1)
            {
                return GetRandomString(Random.Range(0, st.Length));
            }

            string result = "";
            for (int i=0; i<len; i++)
            {
                char c = (char)('A' + Random.Range(0, st.Length));
                result += c;
            }
            return result;
        }
    }
}