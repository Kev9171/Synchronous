using TMPro;
using UnityEngine;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class BuffInfoPanel : MonoBehaviour
    {
        [SerializeField]
        TMP_Text exLabel;

        public void Init()
        {
            //empty
        }

        /// <summary>
        /// 누르고 있을 때만 보이도록 하므로 아래 내용 필요 x
        /// </summary>
        public void OnClickClose()
        {
            // empty
        }

        public void SetData(string ex)
        {
            exLabel.text = ex;
        }

        public void SetData(BuffBase bb)
        {
            exLabel.text = bb.explanation;
        }
    }
}
