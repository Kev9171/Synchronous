using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeploymentManager : MonoBehaviour
{
    public CharacterBtn ClickedBtn { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickedCharacter(CharacterBtn characterBtn)
    {
        this.ClickedBtn = characterBtn;
    }

    public void DeployLimit()
    {
        ClickedBtn = null;
    }
}
