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

        private void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(OnClicked);
        }

        public void OnClicked()
        {
            GameObject pick = GameObject.Find("PickControl");
            if (NullCheck.IsGameObjectNull(pick)) { return; }

            if (!NullCheck.HasItComponent<PickControl>(pick, "PickControl")) { return; }

            pick.GetComponent<PickControl>().OnCharaSelected(cid);
        }
    }
}

