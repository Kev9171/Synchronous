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
            // ������ ���� ok ���� ���� �� ȣ�� 
            if (state)
            {
                // ��ư �� �̻� ������ ���ϵ���
                this.GetComponent<Button>().interactable = false;
            }
            else
            {
                // ���� �����ֱ�
                PanelBuilder.ShowFadeOutText(UITransform, "Can not read value from the server...");

                // �ٽ� �������
                this.GetComponent<Button>().interactable = true;
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
