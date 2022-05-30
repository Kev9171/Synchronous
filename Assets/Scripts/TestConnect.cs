using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        print("Connecting to server.");
        PhotonNetwork.AutomaticallySyncScene = true; // 사용자 간 가장 쉬운 loading scene 방법
        PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // print("Connected to server.");
        // print(PhotonNetwork.LocalPlayer.NickName);
        Debug.Log("Connected to Photon.", this);
        Debug.Log("My Nickname is " + PhotonNetwork.LocalPlayer.NickName, this);
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // print("Disconnected from server for reason " + cause.ToString());
        Debug.Log("Failed to connect to Photon: " + cause.ToString(), this);
    }

    public override void OnJoinedLobby()
    {
        print("Joined lobby.");
    }
}
