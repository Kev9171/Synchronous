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

        GameObject buffInfoPanel;

        private int turn;
        private BuffBase buffBase;

        public void SetData(BuffBase bb, int turn)
        {
            icon.sprite = bb.icon;
            turnLabel.text = turn.ToString();
            this.turn = turn;

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

        public void ButtonUp()
        {
            //buffInfoPanel.SetActive(false);
            Destroy(buffInfoPanel);
        }

        public void ButtonDown()
        {
            GameObject canvas = GameObject.Find("UICanvas");
            buffInfoPanel =  PanelBuilder.ShowBuffInfoPanel(canvas.transform, buffBase);
            //buffInfoPanel.GetComponent<BuffInfoPanel>().SetData(buffBase.explanation);
            //buffInfoPanel.SetActive(true);
        }

        #region MonoBehaviour CallBacks

        private void Start()
        {
            buffInfoPanel = GameObject.FindGameObjectWithTag("BuffInfoPanel");
        }

        #endregion
    }
}
