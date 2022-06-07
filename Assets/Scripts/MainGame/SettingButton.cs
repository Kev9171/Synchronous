using UnityEngine.UI;
using UnityEngine;

namespace KWY
{
    [RequireComponent(typeof(Button))]
    public class SettingButton : MonoBehaviour
    {
        [SerializeField]
        GameObject settingPanel;
        public void OnClickSettingBtn()
        {
            settingPanel.SetActive(true);
        }
    }
}
