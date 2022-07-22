using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TMPro;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class InputPopupPanel : MonoBehaviour, IInstantiatableUI
    {
        [SerializeField]
        TMP_Text contentText;

        [SerializeField]
        TMP_InputField inputField;

        [SerializeField]
        Button okBtn;

        public void SetData(string content, UnityAction<string> callback, int maxInputChar)
        {
            contentText.text = content;
            okBtn.onClick.AddListener(delegate { callback(inputField.text); });
            inputField.characterLimit = maxInputChar;
        }

        public void Init()
        {
            //
        }

        public void OnClickClose()
        {
            Destroy(gameObject);
        }
    }
}
