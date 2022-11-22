using System;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    class CharacterObserver : IObserver<Character>
    {
        CharacterUIHandler _characterUIHandler;

        CharacterUIHandler CharacterUIHandler
        {
            get
            {
                if (!_characterUIHandler)
                {
                    FindCharacterUIHandler();
                }
                return _characterUIHandler;
            }
        }

        public void OnNotify(Character t)
        {
            UpdateData(t);
        }

        public void UpdateData(Character t)
        {
            CharacterHpBarController.Instance.UpdateHp(t); // 먼저 호출... 필요
            CharacterUIHandler.UpdateCharacterStatusUI(t);
        }

        private void FindCharacterUIHandler()
        {
            GameObject g = GameObject.Find("UIHandler");

            if (!g)
            {
                Debug.LogError($"Can not find gameobject named: 'CharacterUIHandler'");
            }

            _characterUIHandler = g.GetComponent<CharacterUIHandler>();

            if (!_characterUIHandler)
            {
                Debug.LogError("Can not find component in CharacterUIHandler : 'CharacterUIHandler'");
            }
        }
    }
}
