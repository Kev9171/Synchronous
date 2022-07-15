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
        /// ������ ���� ���� ���̵��� �ϹǷ� �Ʒ� ���� �ʿ� x
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
