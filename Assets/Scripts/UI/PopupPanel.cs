using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class PopupPanel : MonoBehaviour
    {
        [SerializeField]
        TMP_Text ContentText;

        [SerializeField]
        Button OkBtn;

        public void SetData(string content)
        {
            ContentText.text = content;
            OkBtn.onClick.AddListener(OnClickOkBtn);
        }

        public void SetData(string content, UnityAction btnCallback, bool destoryGameobject)
        {
            ContentText.text = content;
            OkBtn.onClick.AddListener(btnCallback);

            if (destoryGameobject)
            {
                OkBtn.onClick.AddListener(OnClickOkBtn);
            }
        }

        public void OnClickOkBtn()
        {
            Destroy(gameObject);
        }
    }
}
