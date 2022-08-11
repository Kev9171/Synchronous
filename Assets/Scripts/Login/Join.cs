using UnityEngine;
using UnityEngine.UI;

using TMPro;

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

        #region Private Fields

        private bool[] JoinCondition = new bool[4];
        const string pwReg = "";

        #endregion

        #region Constant Fields

        const string IdCheckFailed = "Unavaiable Id, check again!";
        const string WelcomeUser = "Join completed!\n\n Yout UID: ";
        const string JoinFailed = "Joining is not available now.";
        const string NameCheckFailed = "Unavaiable Name, check again!";

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
            // todo

            bool ok = true; // temp


            // ���Խ� Ȯ��

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
            if (PwInput.text == PwCheckInput.text)
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
            string id = IdInput.text;

            // for test
            CheckIdCallback(true);

            // original code
            //StartCoroutine(LoginJoinAPI.Instance.IdCheckPost(id, CheckIdCallback));
        }

        public void OnClickNameCheckBtn()
        {
            string name = NameInput.text;

            // for test
            CheckNameCallback(true);

            // original code
            //StartCoroutine(LoginJoinAPI.Instance.IdCheckPost(name, CheckNameCallback));
        }

        public void OnClickJoinBtn()
        {
            // for test
            JoinCallback(true, "111");

            // original code
            /*string id = IdInput.text;
            string name = NameInput.text;
            string pw = PwInput.text;

            StartCoroutine(LoginJoinAPI.Instance.JoinPost(id, name, pw, JoinCallback));*/
        }

        public void JoinedCompleteCallback()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region Callback methods for LoginJoinAPI

        public void CheckIdCallback(bool message)
        {
            if (message)
            {
                IdCheckImg.sprite = CheckOkImg;
                JoinCondition[0] = true;

                CheckJoinable();
            }
            else
            {
                IdCheckImg.sprite = CheckNoImg;
                JoinCondition[0] = false;

                JoinBtn.interactable = false;

                // �˾�â ����
                GameObject canvas = GameObject.Find("Canvas");
                PopupBuilder.ShowPopup(canvas.transform, IdCheckFailed);
            }
        }

        public void CheckNameCallback(bool message)
        {
            if (message)
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
                GameObject canvas = GameObject.Find("Canvas");
                PopupBuilder.ShowPopup(canvas.transform, NameCheckFailed);
            }
        }

        public void JoinCallback(bool message, string uid)
        {
            if (message)
            {
                GameObject canvas = GameObject.Find("Canvas");
                PopupBuilder.ShowPopup(canvas.transform, WelcomeUser + uid, JoinedCompleteCallback, true);

                SelectPanel.SetActive(true);
            }
            else
            {
                GameObject canvas = GameObject.Find("Canvas");
                PopupBuilder.ShowPopup(canvas.transform, JoinFailed);
            }
        }

        #endregion

        private void OnEnable()
        {
            // clear field
            IdInput.text = "";
            NameInput.text = "";
            PwInput.text = "";
            PwCheckInput.text = "";

            JoinBtn.interactable = false;
            PwCheckInput.interactable = false;
        }
    }
}
