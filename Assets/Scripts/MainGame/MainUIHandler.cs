using System;
using System.Collections;
using UnityEngine;

namespace KWY
{
    public class MainUIHandler : MonoBehaviour
    {
        [SerializeField]
        CharacterUIHandler _characterUIHandler;

        CharacterUIHandler CharaUI
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

        private void FindCharacterUIHandler()
        {
            GameObject g = GameObject.Find("CharacterUIHandler");
            if (!g)
            {
                _characterUIHandler = g.GetComponent<CharacterUIHandler>();

                if (!_characterUIHandler)
                {
                    Debug.Log($"Can not find component: 'CharacterUI' in {g}");
                }
            }
            else
            {
                Debug.Log("Can not find GameObject named 'CharacterUIHandler'");
            }
        }
    }
}