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
        TMP_InputField PwInput;

        [SerializeField]
        TMP_InputField PwCheckInput;

        [SerializeField]
        Button IdCheckBtn;

        [SerializeField]
        Image IdCheckImg;

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

        private bool[] JoinCondition = new bool[3];

        #endregion

        #region Constant Fields

        const string IdCheckFailed = "Unavaiable Id, check again!";
        const string WelcomeUser = "Join completed! Login please.";

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

        public void OnEndEditPw()
        {
            bool ok = true; // temp
            // ���Խ� Ȯ��

            // ������ again pw input field Ȱ��ȭ
            if (ok)
            {
                PwCheckInput.interactable = true;
                JoinCondition[1] = true;
                PwCheckImg.sprite = CheckOkImg;

                CheckJoinable();
            }
            else
            {
                PwCheckInput.interactable = false;
                JoinCondition[1] = false;
                PwCheckImg.sprite = CheckNoImg;
            }
        }

        public void OnPwAgainValueChagned()
        {
            // �� ��� ��ȣ Ȯ��
            if (PwInput.text == PwCheckInput.text)
            {
                JoinCondition[2] = true;
                PwAgainCheckImg.sprite = CheckOkImg;

                CheckJoinable();
            }
            else
            {
                JoinCondition[2] = false;
                PwAgainCheckImg.sprite = CheckNoImg;
            }
        }

        #endregion

        #region Button Elements Callbacks

        public void OnClickIdCheckBtn()
        {
            string id = IdInput.text;

            // ������ ���� ��� ������ ���̵����� üũ 
            //bool ok = ;

            bool ok = true; // temp

            if (ok)
            {
                IdCheckImg.sprite = CheckOkImg;
                JoinCondition[0] = true;

                CheckJoinable();
            }
            else
            {
                IdCheckImg.sprite = CheckNoImg;
                JoinCondition[0] = false;

                // �˾�â ����
                GameObject canvas = GameObject.Find("Canvas");
                PopupBuilder.ShowPopup(canvas.transform, IdCheckFailed);
            }
        }

        public void OnClickJoinBtn()
        {
            GameObject canvas = GameObject.Find("Canvas");
            PopupBuilder.ShowPopup(canvas.transform, WelcomeUser, JoinedCompleteCallback, true);

            SelectPanel.SetActive(true);
        }

        public void JoinedCompleteCallback()
        {
            gameObject.SetActive(false);
        }

        #endregion

        private void OnEnable()
        {
            // clear field
            IdInput.text = "";
            PwInput.text = "";
            PwCheckInput.text = "";

            JoinBtn.interactable = false;
            PwCheckInput.interactable = false;
        }
    }
}
