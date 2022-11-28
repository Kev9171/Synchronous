using System;

using UnityEngine;

namespace KWY
{
    public class PlayerObserver : IObserver
    {
        PlayerUIHandler _playerUIHandler;

        PlayerUIHandler PlayerUIHandler
        {
            get
            {
                if (!_playerUIHandler)
                {
                    FindPlayerUIHandler();
                }
                return _playerUIHandler;
            }
        }

        public void OnNotify()
        {
            UpdateData();
        }

        public void UpdateData()
        {
            PlayerUIHandler.UpdatePlayerMpPanel();
        }

        private void FindPlayerUIHandler()
        {
            GameObject g = GameObject.Find("UIHandler");

            if (!g)
            {
                Debug.LogError($"Can not find gameobject named: 'PlayerUIHandler'");
            }

            _playerUIHandler = g.GetComponent<PlayerUIHandler>();

            if (!_playerUIHandler)
            {
                Debug.LogError("Can not find component in PlayerUIHandler : 'PlayerUIHandler'");
            }
        }
    }
}
