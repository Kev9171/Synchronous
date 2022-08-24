using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBtn : MonoBehaviour
{
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private int chance;
    [SerializeField]
    Button[] button = new Button[3];

    public GameObject CharacterPrefab
    {
        get
        {
            return characterPrefab;
        }
    }

    public int Chance
    {
        get
        {
            return chance;
        }
    }
}
