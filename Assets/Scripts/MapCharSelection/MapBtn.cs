using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBtn : MonoBehaviour
{
    [SerializeField] private GameObject mapPrefab;
    public GameObject MapPrefab
    {
        get
        {
            return mapPrefab;
        }
    }
}
