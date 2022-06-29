using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyTemp : MonoBehaviour
{
    [SerializeField]
    GameObject startBtn;

    [SerializeField]
    GameObject readyBtn;


    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            startBtn.SetActive(false);
        }
        startBtn.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
