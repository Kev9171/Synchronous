using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleManager : MonoBehaviour
{
    public Night night;

    public void NA_clicked()
    {
        night.NormalAttack();
    }

    public void SA_clicked()
    {
        night.SpecialAttack();
    }
}
