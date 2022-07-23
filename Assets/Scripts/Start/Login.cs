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

        string loginFailedMsg = "User no found";
        string loginConditionFailedMsg = "Enter at least one word for id and password";

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

            if(ok)
            {
                Debug.Log("Login Successed");

                GameObject mainPanel = GameObject.Find("MainPanel");
                if (mainPanel == null)
                {
                    Debug.LogError("Can not find the gameobject named mainPanel");
                    return;
                }
                mainPanel.SetActive(true);
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
