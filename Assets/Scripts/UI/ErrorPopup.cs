using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

namespace KWY
{
    public class ErrorPopup : MonoBehaviour, IInstantiatableUI
    {
        [SerializeField]
        TMP_Text ErrorContentText;

        [SerializeField]
        TMP_Text ErrorCodeText;

        [SerializeField]
        Button OkBtn;

        public void SetData(ErrorCode eCode)
        {
            string s = ErrorMsg.Instance.eData.getMsg(eCode);

            if (s == null)
            {
                // error
            }
            else
            {
                ErrorCodeText.text = eCode.ToString();
                ErrorContentText.text = s;
            }

            OkBtn.onClick.AddListener(OnClickClose);
        }

        public void SetData(ErrorCode eCode, UnityAction btnCallback)
        {
            string s = ErrorMsg.Instance.eData.getMsg(eCode);

            if (s == null)
            {
                // error
            }
            else
            {
                ErrorCodeText.text = eCode.ToString();
                ErrorContentText.text = s;
            }

            OkBtn.onClick.AddListener(btnCallback);
        }

        public void Init()
        {
            ;
        }

        public void OnClickClose()
        {
            Destroy(gameObject);
        }
    }
}
