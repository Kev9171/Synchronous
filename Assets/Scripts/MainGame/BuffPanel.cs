using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class BuffPanel : MonoBehaviour
    {
        [SerializeField]
        Image icon;

        [SerializeField]
        TMP_Text turnLabel;

        [Tooltip("buff ������ ���� UI Panel; Character Panel �κ��� �� �Ҵ� �޾Ƽ� ���")]
        GameObject BuffInfoPanel;

        private int turn;
        private BuffBase buffBase;

        public void SetData(BuffBase bb, int turn, GameObject infoPanel)
        {
            icon.sprite = bb.icon;
            turnLabel.text = turn.ToString();
            this.turn = turn;

            BuffInfoPanel = infoPanel;

            buffBase = bb;
        }

        /// <summary>
        /// ���� ���� ǥ���ϴ� ���ڸ� ������Ʈ�ϴ� �Լ�; false ��ȯ �� destry ���־�ߵ�
        /// </summary>
        /// <param name="reduce">���ҽ�ų ���� ��</param>
        /// <returns>���� �� ���� 0�� �ɰ�� false ��ȯ</returns>
        public bool ReduceBuffTurnText(int reduce)
        {
            turn -= reduce;

            if (turn <= 0)
            {
                turnLabel.text = "0";
                return false;
            }

            turnLabel.text = turn.ToString();
            return true;
        }

        #region MonoBehaviour CallBacks

        private void Start()
        {
            BuffInfoPanel.SetActive(false);
        }

        #endregion

        #region MouseEventBase

        // ���콺 Ŭ�� ���� ���� ���� ���̰� �ϱ�

        /// <summary>
        /// ���콺 Ŭ�� ���� ��
        /// </summary>
        private void OnMouseDown()
        {
            BuffInfoPanel.GetComponent<BuffInfoPanel>().SetText(buffBase.explanation);
            BuffInfoPanel.SetActive(true);
        }

        /// <summary>
        /// ���콺 Ŭ���� ��ҵǾ��� ��
        /// </summary>
        private void OnMouseUp()
        {
            BuffInfoPanel.SetActive(false);
        }

        #endregion

    }
}
