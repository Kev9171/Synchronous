using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharBtn : MonoBehaviour
{
    [SerializeField] private GameObject charPrefab;
    public GameObject CharPrefab
    {
        get
        {
            return charPrefab;
        }
    }
}
