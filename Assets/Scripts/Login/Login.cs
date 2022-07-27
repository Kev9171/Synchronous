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

        string loginFailedMsg = "User no found";
        string loginConditionFailedMsg = "Enter at least one word for id and password";
        string loginSuccessMsg = "Welcome to Synchronous!";

        public void OnClickLoginBtn()
        {
            string id = IdInput.text;
            string pw = PwInput.text;

            if (id == "" || pw == "")
            {
                GameObject canvas = GameObject.Find("Canvas");
                PopupBuilder.ShowPopup(canvas.transform, loginConditionFailedMsg);
                return;
            }

            // LoginJoinAPI 사용하여 로그인 확인
            bool ok = true;

            // db 로부터 받은 데이터를 UserManager에 저장
            // 아래 값은 임시
            string accountId = "temp_id";
            Sprite userIcon = null;
            int userLevel = 1;
            int userId = 12345;
            UserManager.InitData(userIcon, accountId, userLevel, userId);

            if(ok)
            {
                Debug.Log("Login Successed");

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
    }
}
