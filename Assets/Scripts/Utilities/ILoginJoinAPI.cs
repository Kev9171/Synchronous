using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KWY
{
    public interface ILoginJoinAPI
    {
        IEnumerator LoginPost(string id, string pw, UnityAction<LoginResData> callback, UnityAction<ErrorCode> errorCallback);
        IEnumerator IdCheckPost(string id, UnityAction<IdCheckResData> callback, UnityAction<ErrorCode> errorCallback);
        IEnumerator NameCheckPost(string name, UnityAction<NameCheckResData> callback, UnityAction<ErrorCode> errorCallback);
        IEnumerator JoinPost(string id, string name, string pw, UnityAction<JoinResData> callback, UnityAction<ErrorCode> errorCallback);
        IEnumerator LogoutPost(string id, UnityAction<LogoutResData> callback, UnityAction<ErrorCode> errorCallback);
    }
}
