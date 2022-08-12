using UnityEngine;
using UnityEngine.UI;

using TMPro;

using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class Join : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField IdInput;

        [SerializeField]
        TMP_InputField NameInput;

        [SerializeField]
        TMP_InputField PwInput;

        [SerializeField]
        TMP_InputField PwCheckInput;

        [SerializeField]
        Button IdCheckBtn;

        [SerializeField]
        Button NameCheckBtn;

        [SerializeField]
        Image IdCheckImg;

        [SerializeField]
        Image NameCheckImg;

        [SerializeField]
        Image PwCheckImg;

        [SerializeField]
        Image PwAgainCheckImg;

        [SerializeField]
        Button JoinBtn;

        [SerializeField]
        Sprite CheckOkImg;

        [SerializeField]
        Sprite CheckNoImg;

        [SerializeField]
        GameObject SelectPanel;

        [SerializeField]
        Transform CanvasTransform;

        [SerializeField]
        GameObject LoadingPanel;

        #region Private Fields

        private bool[] JoinCondition = new bool[4];
        Regex pwRegex = new Regex(@"[a-zA-Z0-9]{5,15}$");

        #endregion

        #region Constant Fields

        const string IdCheckFailed = "Unavaiable Id, check again!";
        const string WelcomeUser = "Join completed!\n\n Yout UID: ";
        const string JoinFailed = "Joining is not available now.";
        const string NameCheckFailed = "Unavaiable Name, check again!";
        const string NotEmailId = "Id should be email-format.";
        string inputFailedMsg = "Enter at least one word for id and password";

        #endregion


        #region Private Methods 

        /// <summary>
        /// pw �ʵ� ���� ����ǰų� id üũ ���� ����� �� ȣ�� �ʿ�
        /// </summary>
        private void CheckJoinable()
        {
            // ���� Ȯ��
            foreach (bool ch in JoinCondition)
            {
                if (!ch)
                {
                    return;
                }
            }

            // ���� ok ������ ��� ��ư Ȱ��ȭ
            JoinBtn.interactable = true;
        }

        #endregion

        #region Input Field Elements Callbacks

        public void OnPwValueChanged()
        {
            string pw = PwInput.text.Trim();

            bool ok = pwRegex.IsMatch(pw);

            // ������ again pw input field Ȱ��ȭ
            if (ok)
            {
                PwCheckInput.interactable = true;
                JoinCondition[2] = true;
                PwCheckImg.sprite = CheckOkImg;

                CheckJoinable();
            }
            else
            {
                PwCheckInput.interactable = false;
                JoinCondition[2] = false;
                PwCheckImg.sprite = CheckNoImg;

                JoinBtn.interactable = false;
            }

            if (PwInput.text != PwCheckInput.text)
            {
                JoinCondition[3] = false;
                PwAgainCheckImg.sprite = CheckNoImg;
            }
        }

        public void OnPwAgainValueChagned()
        {
            // �� ��� ��ȣ Ȯ��
            if (PwInput.text.Trim() == PwCheckInput.text.Trim())
            {
                JoinCondition[3] = true;
                PwAgainCheckImg.sprite = CheckOkImg;

                CheckJoinable();
            }
            else
            {
                JoinCondition[3] = false;
                PwAgainCheckImg.sprite = CheckNoImg;

                JoinBtn.interactable = false;
            }
        }

        public void OnIdValueChanged()
        {
            // �ٽ� Ȯ���ؾ� �ϹǷ� �ʱ�ȭ
            IdCheckImg.sprite = CheckNoImg;
            JoinCondition[0] = false;
        }

        public void OnNameValueChanged()
        {
            // �ٽ� Ȯ���ؾ� �ϹǷ� �ʱ�ȭ
            NameCheckImg.sprite = CheckNoImg;
            JoinCondition[1] = false;
        }

        #endregion

        #region Button Elements Callbacks

        public void OnClickIdCheckBtn()
        {
            string id = IdInput.text.Trim();

            try
            {
                MailAddress m = new MailAddress(id);
            }
            catch(Exception)
            {
                // email �ƴ�
                // �˾�â ����
                PopupBuilder.ShowPopup(CanvasTransform, NotEmailId);
                return;
            }

            ShowLoadingPanel();

            // for test
            //CheckIdCallback(new IdCheckResData((int)ResCode.TRUE, "OK"));

            // original code
            StartCoroutine(LoginJoinAPI.Instance.IdCheckPost(id, CheckIdCallback, ErrorCallback));
        }

        public void OnClickNameCheckBtn()
        {
            string name = NameInput.text.Trim();

            if (name == "")
            {
                PopupBuilder.ShowPopup(CanvasTransform, inputFailedMsg);
                return;
            }

            ShowLoadingPanel();

            // for test
            //CheckNameCallback(new NameCheckResData((int)ResCode.TRUE, "OK"));

            // original code
            StartCoroutine(LoginJoinAPI.Instance.NameCheckPost(name, CheckNameCallback, ErrorCallback));
        }

        public void OnClickJoinBtn()
        {
            ShowLoadingPanel();

            // for test
            //JoinCallback(new JoinResData((int)ResCode.TRUE, "OK", 111));

            // original code
            string id = IdInput.text.Trim();
            string name = NameInput.text.Trim();
            string pw = PwInput.text.Trim();

            StartCoroutine(LoginJoinAPI.Instance.JoinPost(id, name, pw, JoinCallback, ErrorCallback));
        }

        public void JoinedCompleteCallback()
        {
            SelectPanel.SetActive(true);
            gameObject.SetActive(false);
        }

        #endregion

        #region Callback methods for LoginJoinAPI

        public void CheckIdCallback(IdCheckResData data)
        {
            HideLoadingPanel();

            int code = data.code;

            // ������ ���� �����޼��� �ްų� ������ ���� ���
            if (code == (int)ResCode.ERROR)
            {
                // �ϴ� log ��...
                Debug.LogError(data.message);
            }
            // ��� ���� ���̵��� ���
            else if (code == (int)ResCode.TRUE)
            {
                IdCheckImg.sprite = CheckOkImg;
                JoinCondition[0] = true;

                CheckJoinable();
            }
            // �ߺ� ���̵��� ���
            else
            {
                IdCheckImg.sprite = CheckNoImg;
                JoinCondition[0] = false;

                JoinBtn.interactable = false;

                // �˾�â ����
                PopupBuilder.ShowPopup(CanvasTransform, IdCheckFailed);
            }
        }

        public void CheckNameCallback(NameCheckResData data)
        {
            HideLoadingPanel();

            int code = data.code;

            if (code == (int)ResCode.ERROR)
            {
                // �ϴ� log ��...
                Debug.LogError(data.message);
            }
            else if (code == (int)ResCode.TRUE)
            {
                NameCheckImg.sprite = CheckOkImg;
                JoinCondition[1] = true;

                CheckJoinable();
            }
            else
            {
                NameCheckImg.sprite = CheckNoImg;
                JoinCondition[1] = false;

                JoinBtn.interactable = false;

                // �˾�â ����
                PopupBuilder.ShowPopup(CanvasTransform, NameCheckFailed);
            }
        }

        public void JoinCallback(JoinResData data)
        {
            HideLoadingPanel();

            int code = data.code;

            if (code == (int)ResCode.ERROR)
            {
                // �ϴ� log ��...
                Debug.LogError(data.message);
            }
            else if (code == (int)ResCode.TRUE)
            {
                PopupBuilder.ShowPopup(CanvasTransform, WelcomeUser + data.uid, JoinedCompleteCallback, true);
            }
            else
            {
                PopupBuilder.ShowPopup(CanvasTransform, JoinFailed);
            }
        }

        public void ErrorCallback(ErrorCode code)
        {
            HideLoadingPanel();

            PopupBuilder.ShowErrorPopup(CanvasTransform, code);
        }

        #endregion

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
            // clear field
            IdInput.text = "";
            NameInput.text = "";
            PwInput.text = "";
            PwCheckInput.text = "";

            JoinBtn.interactable = false;
            PwCheckInput.interactable = false;

            LoadingPanel.SetActive(false);
        }
    }
}
