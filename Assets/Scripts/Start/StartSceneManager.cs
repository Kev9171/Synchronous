using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class StartSceneManager : MonoBehaviour
    {
        [SerializeField]
        GameObject MenuPanel1;

        [SerializeField]
        GameObject MenuPanel2;

        [SerializeField]
        Image UserIcon;

        [SerializeField]
        TMP_Text UserName;

        [SerializeField]
        ConnectPhoton connectPhoton;

        [SerializeField]
        Transform CanvasTransform;

        const string createRoomContent = "Enter the name of room you want to create.";
        const string joinRoomContent = "Enter the name of room you want to join.";
        const string gameInfoContent = "ver 1.0.0";
        public void LoadUserInfo()
        {
            UserIcon.sprite = UserManager.UserIcon;
            UserName.text = UserManager.AccountId;
        }

        #region Button Callbacks

        public void OnStartGameBtnClicked()
        {
            MenuPanel1.SetActive(false);
            MenuPanel2.SetActive(true);
        }

        public void OnBackBtnClicked()
        {
            MenuPanel2.SetActive(false);
            MenuPanel1.SetActive(true);
        }

        public void OnCreateRoomBtnClicked()
        {
            PopupBuilder.ShowInputPopup(CanvasTransform, createRoomContent, OnCreateRoomBtnCallback);
        }

        public void OnJoinRoomBtnClicked()
        {
            PopupBuilder.ShowInputPopup(CanvasTransform, joinRoomContent, OnJoinRoomBtnCallback);
        }

        public void OnRoomListBtnClicked()
        {
            PopupBuilder.ShowRoomListPopup(CanvasTransform, null);
        }

        public void OnSettingBtnClicked()
        {
            PopupBuilder.ShowSettingPanel(CanvasTransform, null);
        }

        public void OnGameInfoBtnClicked()
        {
            PopupBuilder.ShowPopup(CanvasTransform, gameInfoContent);
        }

        public void OnCreateRoomBtnCallback(string roomName)
        {
            connectPhoton.CreateRoom(roomName);
        }

        public void OnJoinRoomBtnCallback(string roomName)
        {
            connectPhoton.JoinNamedRoom(roomName);
        }

        #endregion

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            LoadUserInfo();

            MenuPanel2.SetActive(false);
        }

        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                // connect photon and 로비 진입
                connectPhoton.ConnectPhotonServer();
            }
        }

        #endregion
    }
}
