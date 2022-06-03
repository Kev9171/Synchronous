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

        #region MonoBehaviour CallBacks

        private void Start()
        {
            BuffInfoPanel.SetActive(false);
        }

        #endregion

        #region MouseEventBase

        // 마우스 클릭 중일 때만 설명 보이게 하기

        /// <summary>
        /// 마우스 클릭 했을 때
        /// </summary>
        private void OnMouseDown()
        {
            BuffInfoPanel.GetComponent<BuffInfoPanel>().SetText(buffBase.explanation);
            BuffInfoPanel.SetActive(true);
        }

        /// <summary>
        /// 마우스 클릭이 취소되었을 때
        /// </summary>
        private void OnMouseUp()
        {
            BuffInfoPanel.SetActive(false);
        }

        #endregion

    }
}
