using KWY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBtn : MonoBehaviour
{
    [SerializeField] private GameObject characterPrefab;

    public GameObject CharacterPrefab
    {
        get
        {
            return characterPrefab;
        }
    }
}