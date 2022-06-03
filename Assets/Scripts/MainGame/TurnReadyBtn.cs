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


        // ready ������ ���� ���� ���� (text �� �ٲ�)
        public void SendReadyState()
        {
            // ready �Ǿ��� �� ����
            mainGameEvent.RaiseEventTurnReady();

            // ready ��ư �̹��� ���� or �ؽ�Ʈ ����
            ReadyBtn.GetComponent<Image>().color = IsReadyColor;
        }

        public void SetReady(bool state)
        {
            // ������ ���� ok ���� ���� �� ȣ�� 
            if (state)
            {
                // ��ư �� �̻� ������ ���ϵ���
                ReadyBtn.interactable = false;

                // cancel �� �ؽ�Ʈ �ٲٱ�
                ReadyText.text = "Cancel";
            }
            else
            {
                // �ٽ� �������
                ReadyBtn.GetComponent<Image>().color = Color.white;
            }
        }

        public void ResetReady()
        {
            // �ٽ� �������
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
