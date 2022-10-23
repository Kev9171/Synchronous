using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickScene
{
    public class DontDestoryObject : MonoBehaviour
    {
        private void Awake()
        {
            var obj = FindObjectsOfType<DontDestoryObject>();
            if (obj.Length == 1)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

