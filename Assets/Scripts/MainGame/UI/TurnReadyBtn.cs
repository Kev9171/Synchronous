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
            // ������ ���� ok ���� ���� �� ȣ�� 
            if (state)
            {
                // ��ư �� �̻� ������ ���ϵ���
                this.GetComponent<Button>().interactable = false;

                // cancel �� �ؽ�Ʈ �ٲٱ�
                ReadyText.text = "Cancel";
            }
            else
            {
                // �ٽ� �������
                this.GetComponent<Image>().color = Color.white;
            }
        }

        public void ResetReady()
        {
            // �ٽ� �������
            this.GetComponent<Image>().color = Color.white;
            this.GetComponent<Button>().interactable = true;
        }
    }
}
