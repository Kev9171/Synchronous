using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace KWY
{
    [RequireComponent(typeof(Button))]
    public class LogoutBtn : MonoBehaviour
    {
        [SerializeField]
        Transform CanvasTransform;

        string OkMsg = "Log out";
        string LoginLevel = "LoginScene";
        string LogoutQuestion = "Do you want to log out?";

        public void OnBtnClicked()
        {
            PopupBuilder.ShowPopup2(CanvasTransform, LogoutQuestion, LogoutOkCallback);
        }

        // ok 눌렀을 경우 로그아웃 실행
        public void LogoutOkCallback()
        {
            StartCoroutine(LoginJoinAPI.Instance.LogoutPost(UserManager.AccountId, ShowOkPopup, ErrorCallback));
        }

        // 성공적으로 로그아웃 되었을 때
        public void ShowOkPopup(LogoutResData data)
        {
            Debug.Log(data);
            PopupBuilder.ShowPopup(CanvasTransform, OkMsg, LoadLoginLevel);
        }

        // 로그아웃이 제대로 안되었을 때 (그러나 게임에 영향이 없음)
        public void ErrorCallback(ErrorCode code)
        {
            PopupBuilder.ShowErrorPopup(CanvasTransform, code, LoadLoginLevel);
        }

        public void LoadLoginLevel()
        {
            SceneManager.LoadScene(LoginLevel);
        }
    }
}
