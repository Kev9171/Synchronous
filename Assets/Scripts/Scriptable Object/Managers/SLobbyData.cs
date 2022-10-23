using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby
{
    [CreateAssetMenu(fileName = "OLobbyData", menuName = "OLobbyData")]
    public class SLobbyData : ScriptableObject
    {
        public int startCountDownSeconds;
    }
}
