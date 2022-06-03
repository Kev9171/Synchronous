using TMPro;
using UnityEngine;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class BuffInfoPanel : MonoBehaviour
    {
        [SerializeField]
        TMP_Text exLabel;

        public void SetText(string ex)
        {
            exLabel.text = ex;
        }
    }
}
