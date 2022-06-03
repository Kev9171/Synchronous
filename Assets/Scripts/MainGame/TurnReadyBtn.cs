using UnityEngine.UI;
using UnityEngine;

using TMPro;

namespace KWY
{
    [RequireComponent(typeof(Button))]
    public class TurnReadyBtn : MonoBehaviour
    {
        MainGameEvent mainGameEvent;

        Button ReadyBtn;
        TMP_Text ReadyText;

        Color IsReadyColor = new Color(210, 210, 210);


        // ready 해제는 아직 구현 안함 (text 만 바뀜)
        public void SendReadyState()
        {
            // ready 되었을 때 실행
            mainGameEvent.RaiseEventTurnReady();

            // ready 버튼 이미지 변경 or 텍스트 변경
            ReadyBtn.GetComponent<Image>().color = IsReadyColor;
        }

        public void SetReady(bool state)
        {
            // 서버로 부터 ok 사인 왔을 떄 호출 
            if (state)
            {
                // 버튼 더 이상 누르지 못하도록
                ReadyBtn.interactable = false;

                // cancel 로 텍스트 바꾸기
                ReadyText.text = "Cancel";
            }
            else
            {
                // 다시 원래대로
                ReadyBtn.GetComponent<Image>().color = Color.white;
            }
        }

        public void ResetReady()
        {
            // 다시 원래대로
            ReadyBtn.GetComponent<Image>().color = Color.white;
            ReadyBtn.interactable = true;
        }

        #region MonoBehaviour CallBacks

        private void OnEnable()
        {
            ReadyBtn.onClick.RemoveAllListeners();
            ReadyBtn.onClick.AddListener(SendReadyState);
        }

        #endregion
    }
}
