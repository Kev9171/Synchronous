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
    }
}
