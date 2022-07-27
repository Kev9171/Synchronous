using UnityEngine;
using UnityEngine.Events;

namespace KWY
{
    public static class PopupBuilder
    {
        public static void ShowPopup(Transform parent, string content)
        {
            GameObject popup = GameObject.Instantiate(
                Resources.Load("Prefabs/UI/PopupPanel",
                typeof(GameObject)
                )) as GameObject;

            popup.transform.SetParent(parent, false);
            popup.GetComponent<PopupPanel>().SetData(content);
        }

        public static void ShowPopup(Transform parent, string content, UnityAction btnCallback, bool destroyGameobject = false)
        {
            GameObject popup = GameObject.Instantiate(
                Resources.Load("Prefabs/UI/PopupPanel",
                typeof(GameObject)
                )) as GameObject;

            popup.transform.SetParent(parent, false);
            popup.GetComponent<PopupPanel>().SetData(content, btnCallback, destroyGameobject);
        }

        public static void ShowPopup2(Transform parent, string content, UnityAction okBtnCallback)
        {
            GameObject popup = GameObject.Instantiate(
                Resources.Load("Prefabs/UI/PopupPanel2",
                typeof(GameObject)
                )) as GameObject;

            popup.transform.SetParent(parent, false);
            popup.GetComponent<PopupPanel2>().SetData(content, okBtnCallback);
        }

        public static void ShowSettingPanel(Transform parent, Object o)
        {
            GameObject settingPanel = GameObject.Instantiate(
                Resources.Load(
                    "Prefabs/UI/SettingPanel",
                    typeof(GameObject)
                    )) as GameObject;

            settingPanel.transform.SetParent(parent, false);
            settingPanel.GetComponent<SettingPanel>().SetData(o);
        }

        public static void ShowInputPopup(Transform parent, string content, UnityAction<string> btnCallBack, int maxInputChar = 10)
        {
            GameObject inputPopupPanel = GameObject.Instantiate(
                Resources.Load(
                    "Prefabs/UI/InputPopupPanel",
                    typeof(GameObject)
                    )) as GameObject;

            inputPopupPanel.transform.SetParent(parent, false);
            inputPopupPanel.GetComponent<InputPopupPanel>().SetData(content, btnCallBack, maxInputChar);
        }

        public static void ShowRoomListPopup(Transform parent, Object o)
        {
            GameObject roomListPanel = GameObject.Instantiate(
                Resources.Load(
                    "Prefabs/UI/RoomListPopup",
                    typeof(GameObject)
                    )) as GameObject;

            roomListPanel.transform.SetParent(parent, false);
            roomListPanel.GetComponent<RoomListingPanel>().SetData(o);
        }
    }
}
