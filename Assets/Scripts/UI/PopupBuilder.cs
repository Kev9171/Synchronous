using UnityEngine;

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
    }
}
