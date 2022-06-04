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

        [Tooltip("buff 정보를 띄우는 UI Panel; Character Panel 로부터 값 할당 받아서 사용")]
        GameObject buffInfoPanel;

        private int turn;
        private BuffBase buffBase;

        public void SetData(BuffBase bb, int turn, GameObject infoPanel)
        {
            icon.sprite = bb.icon;
            turnLabel.text = turn.ToString();
            this.turn = turn;

            buffInfoPanel = infoPanel;

            buffBase = bb;
        }

        /// <summary>
        /// 남은 턴을 표시하는 숫자를 업데이트하는 함수; false 반환 시 destry 해주어야됨
        /// </summary>
        /// <param name="reduce">감소시킬 턴의 수</param>
        /// <returns>남은 턴 수가 0이 될경우 false 반환</returns>
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
            buffInfoPanel.SetActive(false);
        }

        public void ButtonDown()
        {
            buffInfoPanel.GetComponent<BuffInfoPanel>().SetText(buffBase.explanation);
            buffInfoPanel.SetActive(true);
        }

        #region MonoBehaviour CallBacks

        private void Start()
        {
            buffInfoPanel.SetActive(false);
        }

        #endregion
    }
}
