using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class CharacterActionReadyObserver : IObserverCharacter<Character>
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
            CharacterUIHandler.UpdateCharacterActionIcon(t);
        }

        private void FindCharacterUIHandler()
        {
            GameObject g = GameObject.Find("CharacterUIHandler");

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
