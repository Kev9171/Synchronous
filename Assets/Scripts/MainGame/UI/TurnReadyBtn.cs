using UnityEngine.UI;
using UnityEngine;

using TMPro;

namespace KWY
{
    [RequireComponent(typeof(Button))]
    public class TurnReadyBtn : MonoBehaviour
    {
        [SerializeField]
        TMP_Text ReadyText;

        Color IsReadyColor = new Color(210, 210, 210);

        public void SetReady(bool state)
        {
            // 서버로 부터 ok 사인 왔을 떄 호출 
            if (state)
            {
                // 버튼 더 이상 누르지 못하도록
                this.GetComponent<Button>().interactable = false;

                // cancel 로 텍스트 바꾸기
                ReadyText.text = "Cancel";
            }
            else
            {
                // 다시 원래대로
                this.GetComponent<Image>().color = Color.white;
            }
        }

        public void ResetReady()
        {
            // 다시 원래대로
            this.GetComponent<Image>().color = Color.white;
            this.GetComponent<Button>().interactable = true;
        }
    }
}
