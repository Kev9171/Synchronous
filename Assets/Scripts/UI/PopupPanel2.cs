using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class PopupPanel2 : MonoBehaviour
    {
        [SerializeField]
        TMP_Text ContentText;

        [SerializeField]
        Button OkBtn;

        [SerializeField]
        Button CancelBtn;

        public void SetData(string content, UnityAction okBtnCallback)
        {
            ContentText.text = content;
            OkBtn.onClick.AddListener(okBtnCallback);

            CancelBtn.onClick.AddListener(OnClickClose);
        }

        public void OnClickClose()
        {
            Destroy(gameObject);
        }
    }
}
