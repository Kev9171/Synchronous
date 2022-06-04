using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class SettingPanel : MonoBehaviour
    {
        [SerializeField]
        GameObject containerPanel;

        public void OnClickClose()
        {
            containerPanel.SetActive(false);
        }
    }
}
