using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class CharacterActionReadyObserver : IObserver<int>
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

        public void OnNotify(int id)
        {
            CharacterUIHandler.UpdateCharacterActionIcon(id);
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
