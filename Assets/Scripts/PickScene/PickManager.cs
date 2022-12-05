using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
using KWY;
using DebugUtil;

namespace PickScene
{
    public class PickManager : Singleton<PickManager>
    {
        [SerializeField] public float timeLimit = 30;

        [SerializeField] public float timeSecondsToStart = 10;

        [SerializeField]
        Timer timer;

        [SerializeField]
        GameObject timerPanel;

        [SerializeField]
        TMP_Text countDownText;

        private PhotonView photonView;

        private readonly string nextScene = "MainGameScene";

        int readyCount = 0;

        #region Public Methods

        [PunRPC]
        public void ReadyToStartRPC()
        {
            readyCount++;

            if (readyCount == 2)
            {
                StartCountDown();
            }
        }

        public void Timeout()
        {
            PickControl.Instance.RandomDeployCharacter();
            PickControl.Instance.SavePickData();
            PickControl.Instance.StopSelect();

            photonView.RPC("ReadyToStartRPC", RpcTarget.All);
        }

        public void StartCountDown()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            countDownText.text = timeSecondsToStart.ToString();
            timerPanel.gameObject.SetActive(true);

            timer.InitTimer(timeSecondsToStart, StartGame, countDownText);

            timer.StartTimer();
        }

        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                
                PhotonNetwork.LoadLevel(nextScene);
            }
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            photonView = PhotonView.Get(this);

            if (!!NullCheck.HasItComponent<PhotonView>(gameObject, "PhotonView") )
            {
                return;
            }
        }

        #endregion
    }
}
