using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class SettingBtn : MonoBehaviour
    {
        public void OnClickSettingBtn()
        {
            GameObject canvas = GameObject.Find("UICanvas");
            PopupBuilder.ShowSettingPanel(canvas.transform, new Object { });
        }
    }
}
