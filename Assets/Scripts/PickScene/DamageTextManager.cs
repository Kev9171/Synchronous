using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextManager : MonoBehaviour
{
    public GameObject damageTextPrefab, characterInstance;
    public string textToDisplay; // Skill damage

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) // if collision happens
        {
            GameObject DamageTextInstance = Instantiate(damageTextPrefab, characterInstance.transform);
            DamageTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(textToDisplay);
        }
    }
}
