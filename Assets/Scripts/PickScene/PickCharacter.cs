using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Tilemaps;
using KWY;

namespace PickScene
{
    public class PickCharacter : MonoBehaviour 
    {
        [SerializeField] CharacterBase _characterBase;
        public CharacterBase Cb { get; private set; }

        public bool Highlightable { set; get; } = false;
        private Color hightlightColor = Color.red;
        private Color normalColor = Color.white;

        private void Start()
        {
            Cb =  _characterBase;
        }

        private void OnMouseEnter()
        {
            if (Highlightable)
            {
                gameObject.GetComponent<SpriteRenderer>().color = hightlightColor;
            }
        }
        private void OnMouseExit()
        {
            gameObject.GetComponent<SpriteRenderer>().color = normalColor;
        }

        public override string ToString()
        {
            return $"[cid: {Cb.cid}]";
        }
    }
}
