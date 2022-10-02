using System.Collections;
using UnityEngine;

namespace KWY
{
    public class GameProgressObserver : IObserver
    {
        MainUIHandler _mainUiHandler;

        MainUIHandler MainUIHandler
        {
            get
            {
                if (!_mainUiHandler)
                {
                    FindMainUIHandler();
                }
                return _mainUiHandler;
            }
        }

        public void OnNotify()
        {
            UpdateData();
        }

        public void UpdateData()
        {
            MainUIHandler.UpdateTurnText();
        }

        private void FindMainUIHandler()
        {
            GameObject g = GameObject.Find("UIHandler");

            if (!g)
            {
                Debug.LogError($"Can not find gameobject named: 'MainUIHandler'");
            }

            _mainUiHandler = g.GetComponent<MainUIHandler>();

            if (!_mainUiHandler)
            {
                Debug.LogError("Can not find component in MainUIHandler : 'MainUIHandler'");
            }
        }
    }
}