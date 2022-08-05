using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

namespace KWY
{
    public class ErrorPanel : MonoBehaviour, IInstantiatableUI
    {
        [SerializeField]
        TMP_Text ErrorContentText;

        [SerializeField]
        TMP_Text ErrorCodeText;

        [SerializeField]
        Button OkBtn;

        public void SetData(ErrorCode eCode)
        {
            string[] s = ErrorMsg.Instance.eData.GetData(eCode);

            if (s == null)
            {
                // error
            }
            else
            {
                ErrorCodeText.text = s[0];
                ErrorContentText.text = s[1];
            }

            OkBtn.onClick.AddListener(OnClickClose);
        }

        public void SetData(ErrorCode eCode, UnityAction btnCallback)
        {
            string[] s = ErrorMsg.Instance.eData.GetData(eCode);

            if (s == null)
            {
                // error
            }
            else
            {
                ErrorCodeText.text = s[0];
                ErrorContentText.text = s[1];
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
