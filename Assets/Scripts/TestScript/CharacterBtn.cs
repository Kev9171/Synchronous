using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
