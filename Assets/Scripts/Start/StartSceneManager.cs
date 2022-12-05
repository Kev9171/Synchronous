#define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

using UnityEngine.EventSystems;

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

        [SerializeField]
        List<Button> MenuBtns;

        [SerializeField]
        Button LogoutBtn;

        const string createRoomContent = "Enter the name of room you want to create.";
        const string joinRoomContent = "Enter the name of room you want to join.";
        const string roomNameEmptyContent = "Enter more than 1 word.";
        const string reallyQuitGameContent = "Do you want to quit game?";
        public void LoadUserInfo()
        {
            var icon = UserManager.UserIcon;
            if (icon)
            {
                UserIcon.sprite = UserManager.UserIcon;
            }
            
            UserName.text = UserManager.UserName;
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
            string gv = MasterManager.GameSettings.GameVersion;
            string av = MasterManager.GameSettings.AssetVersion;
            string s = string.Format("GameVersion: {0}\nAssetVersion: {1}", gv, av);
            PopupBuilder.ShowPopup(CanvasTransform, s);
        }

        public void OnQuitGameBtnClicked()
        {
            // 게임 종료 전에 저장해야할 데이터 있는 지 확인하는 기능을
            // 포함하는 클래스로 게임 정상 종료 시키기

            // temp
            PopupBuilder.ShowPopup2(CanvasTransform, reallyQuitGameContent, OnQuitOkCallback);
        }

        public void OnQuitOkCallback()
        {
            StartCoroutine(LoginJoinAPI.Instance.LogoutPost(UserManager.AccountId, Application.Quit, Application.Quit));
        }

        public void OnCreateRoomBtnCallback(string roomName)
        {
            if (roomName.Trim() == "")
            {
                PopupBuilder.ShowPopup(CanvasTransform, roomNameEmptyContent);
            }
            else
            {
                connectPhoton.CreateRoom(roomName);
            }
        }

        public void OnJoinRoomBtnCallback(string roomName)
        {
            if (roomName.Trim() == "")
            {
                PopupBuilder.ShowPopup(CanvasTransform, roomNameEmptyContent);
            }
            else
            {
                connectPhoton.JoinNamedRoom(roomName);
            }
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
#if TEST
                ;
#endif
                // connect photon and 로비 진입
                connectPhoton.ConnectPhotonServer();
            }

            foreach(Button b in MenuBtns)
            {
                b.gameObject.AddComponent<MenuBtnEventTrigeer>();
            }

            LogoutBtn.gameObject.AddComponent<MenuBtnEventTrigeer>();
        }

        #endregion

        #region MenuBtn Event Trigeer Callback

        public void OnBtnPointerEnter(Button btn)
        {
            //btn.GetComponent<RectTransform>().localScale = new Vector3(1.05f, 1.05f, 1.05f);
        }

        public void OnBtnPointerExit(Button btn)
        {
            //btn.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        #endregion
    }

    public class MenuBtnEventTrigeer : EventTrigger
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.05f, 1.05f, 1.05f);
            base.OnPointerEnter(eventData);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            base.OnPointerExit(eventData);
        }
    }
}
