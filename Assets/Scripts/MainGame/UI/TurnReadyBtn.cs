using UnityEngine.UI;
using UnityEngine;

using TMPro;

using UI;

namespace KWY
{
    [RequireComponent(typeof(Button))]
    public class TurnReadyBtn : MonoBehaviour
    {
        [SerializeField]
        Transform UITransform;

        public void SetReady(bool state)
        {
            // 서버로 부터 ok 사인 왔을 떄 호출 
            if (state)
            {
                // 버튼 더 이상 누르지 못하도록
                this.GetComponent<Button>().interactable = false;
            }
            else
            {
                // 오류 보여주기
                PanelBuilder.ShowFadeOutText(UITransform, "Can not read value from the server...");

                // 다시 원래대로
                this.GetComponent<Button>().interactable = true;
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
