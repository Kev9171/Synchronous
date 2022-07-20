using UnityEngine;
using UnityEngine.UI;

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
        }

        public void OnClickOkBtn()
        {
            Destroy(gameObject);
        }
    }
}
