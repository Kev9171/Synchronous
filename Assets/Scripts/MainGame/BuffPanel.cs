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
