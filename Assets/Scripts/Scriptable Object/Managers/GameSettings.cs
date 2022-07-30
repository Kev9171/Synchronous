using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Manager/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField]
    private string _gameVersion;
    public string GameVersion
    {
        get
        {
            return _gameVersion;
        }
    }

    [SerializeField]
    private string _assetVersion;
    public string AssetVersion
    {
        get
        {
            return _assetVersion;
        }
    }

    [SerializeField]
    private float _gameLobbyTimerTime;
    public float GameLobbyTimerTime
    {
        get
        {
            return _gameLobbyTimerTime;
        }
    }
}
