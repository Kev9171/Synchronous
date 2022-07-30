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
        /// pw 필드 값이 변경되거나 id 체크 값이 변경될 때 호출 필요
        /// </summary>
        private void CheckJoinable()
        {
            // 조건 확인
            foreach (bool ch in JoinCondition)
            {
                if (!ch)
                {
                    return;
                }
            }

            // 전부 ok 상태일 경우 버튼 활성화
            JoinBtn.interactable = true;
        }

        #endregion

        #region Input Field Elements Callbacks

        public void OnEndEditPw()
        {
            bool ok = true; // temp
            // 정규식 확인

            // 맞으면 again pw input field 활성화
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
            // 위 비밀 번호 확인
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

            // 서버를 통해 사용 가능한 아이디인지 체크 
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

                // 팝업창 띄우기
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
