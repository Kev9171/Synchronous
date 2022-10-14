using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KWY
{
    [RequireComponent(typeof(Button))]
    public class MainGameSettingBtn : MonoBehaviour
    {
        Button button;

        // Start is called before the first frame update
        void Start()
        {
            button = GetComponent<Button>();

            button.onClick.AddListener(OnSettingBtnClicked);
        }

        void OnSettingBtnClicked()
        {
            GameObject canvas = GameObject.Find("UICanvas");
            if (canvas)
            {
                PopupBuilder.ShowSettingPanel(canvas.transform, null);
            }
            else
            {
                Debug.Log("Can not find 'UICanvas' gameobject");
            }

        }
    }
}
