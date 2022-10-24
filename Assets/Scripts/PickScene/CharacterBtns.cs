using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KWY;

namespace PickScene
{
    public class CharacterBtns : MonoBehaviour
    {
        [SerializeField]
        List<CharacterBase> list = new List<CharacterBase>();

        [SerializeField]
        GameObject btnPrefab;

        private void Start()
        {
            foreach(CharacterBase cb in list)
            {
                GameObject o = Instantiate(btnPrefab);
                o.GetComponent<CharacterBtn>().Init(cb.icon, cb.cid);
                o.transform.SetParent(gameObject.transform);
            }
        }
    }

    
}

