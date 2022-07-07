using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class SettingManager : MonoBehaviour
    {
        public void OnClickSettingBtn()
        {
            GameObject canvas = GameObject.Find("UICanvas");
            PanelBuilder.ShowSettingPanel(canvas.transform, new Object{ });
        }
    }
}
