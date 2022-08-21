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

        // ok ������ ��� �α׾ƿ� ����
        public void LogoutOkCallback()
        {
            StartCoroutine(LoginJoinAPI.Instance.LogoutPost(UserManager.AccountId, ShowOkPopup, ErrorCallback));
        }

        // ���������� �α׾ƿ� �Ǿ��� ��
        public void ShowOkPopup(LogoutResData data)
        {
            Debug.Log(data);
            PopupBuilder.ShowPopup(CanvasTransform, OkMsg, LoadLoginLevel);
        }

        // �α׾ƿ��� ����� �ȵǾ��� �� (�׷��� ���ӿ� ������ ����)
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
