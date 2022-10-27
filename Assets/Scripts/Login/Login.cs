#define NO_LOGIN_SERVER

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace KWY
{
    public class Login : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField IdInput;

        [SerializeField]
        TMP_InputField PwInput;

        [SerializeField]
        Button LoginBtn;

        [SerializeField]
        LoginSceneManager loginScene;

        [SerializeField]
        Transform CanvasTransform;

        [SerializeField]
        GameObject LoadingPanel;

        const string loginFailedMsg = "User no found";
        const string loginConditionFailedMsg = "Enter at least one word for id and password";
        const string loginSuccessMsg = "Welcome to Synchronous!";

        public void OnClickLoginBtn()
        {
            string id = IdInput.text.Trim();
            string pw = PwInput.text.Trim();

            if (id == "" || pw == "")
            {
                PopupBuilder.ShowPopup(CanvasTransform, loginConditionFailedMsg);
                return;
            }

            ShowLoadingPanel();

#if NO_LOGIN_SERVER
            LoginCallback(new LoginResData((int)ResCode.TRUE, "OK", 111, "test-email", 1, "temp name", null));
            return;
#endif
            // original code
            StartCoroutine(LoginJoinAPI.Instance.LoginPost(id, pw, LoginCallback, ErrorCallback));
        }

        public void LoginCallback(LoginResData data)
        {
            HideLoadingPanel();

            int code = data.code;

            if (code == (int)ResCode.ERROR)
            {
                // 일단 log 만...
                Debug.LogError(data.message);
            }
            else if (code == (int)ResCode.TRUE)
            {
                Debug.Log("Login Successed");

                // db 로부터 받은 데이터를 UserManager에 저장
                // 아래 값은 임시
                string accountId = data.id;
                Sprite userIcon = null; // 받은 url로 다시 요청해서 이미지 가져와야함@@
                int userLevel = data.level;
                ulong userId = data.uid;
                string userName = data.name;
                UserManager.InitData(userIcon, accountId, userLevel, userId, userName);

                PopupBuilder.ShowPopup(CanvasTransform, loginSuccessMsg,
                    loginScene.AfterLogin, true);
            }
            else
            {
                Debug.Log("Login Failed");
                GameObject canvas = GameObject.Find("Canvas");
                PopupBuilder.ShowPopup(canvas.transform, loginFailedMsg);
            }
        }

        public void ErrorCallback(ErrorCode code)
        {
            HideLoadingPanel();

            PopupBuilder.ShowErrorPopup(CanvasTransform, code);
        }

        private void ShowLoadingPanel()
        {
            LoadingPanel.SetActive(true);
        }

        private void HideLoadingPanel()
        {
            LoadingPanel.SetActive(false);
        }

        private void OnEnable()
        {
            IdInput.text = "";
            PwInput.text = "";

            LoadingPanel.SetActive(false);
        }
    }
}
