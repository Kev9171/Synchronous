using System;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace DebugUtil
{
    public enum LogType
    {
        LOG = 0,
        WARNING = 1,
        ERROR = 2
    }

    public static class DebugLog
    {
        public static void CanNotFindObject(string objectName, LogType logType = 0)
        {
            string log = $"Can not find gameobject named '{objectName}'";

            switch (logType)
            {
                case LogType.LOG:
                    Debug.Log(log);
                    break;
                case LogType.WARNING:
                    Debug.LogWarning(log);
                    break;
                case LogType.ERROR:
                    Debug.Log(log);
                    break;
            }
        }

        public static void CanNotFindComponent(string componentName, string objectName, LogType logType = 0)
        {
            string log = $"Can not find component named '{componentName}' at object '{objectName}'";

            switch (logType)
            {
                case LogType.LOG:
                    Debug.Log(log);
                    break;
                case LogType.WARNING:
                    Debug.LogWarning(log);
                    break;
                case LogType.ERROR:
                    Debug.Log(log);
                    break;
            }
        }

        public static void LogRaiseEvent(byte evcode, object content, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            string str = "";

            if (content is object[] v)
            {
                foreach (object a in v)
                {
                    content += a.ToString() + ", ";
                }
            }

            Debug.Log($"evcode: {evcode}, data: {str}, RaiseEventOptions: {raiseEventOptions}, SendOptions: {sendOptions}");
        }

        public static void FailedToRaiseEvent(byte evcode)
        {
            Debug.LogError($"Failed to send Event: {evcode} to the server");
        }
    }

    public static class NullCheck
    {
        public static bool IsGameObjectNull(GameObject gameObject, LogType logType = 0)
        {
            if (gameObject == null)
            {
                DebugLog.CanNotFindObject(gameObject.name, logType);
                return true;
            }
            else
            {
                return false;
            }
        }

       
        public static bool HasItComponent<T>(GameObject gameObject, string componentName, LogType logType=0)
        {
            if (gameObject.GetComponent<T>() != null)
            {
                return true;
            }
            else
            {
                DebugLog.CanNotFindComponent(componentName, gameObject.name, logType);
                return false;
            }
        }

        public static bool HasItComponent<T>(GameObject gameObject, string componentName, out T component, LogType logType = 0)
        {
            if (gameObject.GetComponent<T>() != null)
            {
                component = gameObject.GetComponent<T>();
                return true;
            }
            else
            {
                DebugLog.CanNotFindComponent(componentName, gameObject.name, logType);
                component = default;
                return false;
            }
        }
    }
}
