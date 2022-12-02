using KWY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DebugUtil;

namespace PickScene
{
    public class CharacterBtn : MonoBehaviour
    {
        CID cid;

        public void Init(Sprite icon, CID cid)
        {
            GetComponent<Image>().sprite = icon;
            this.cid = cid;
        }
    }
}

