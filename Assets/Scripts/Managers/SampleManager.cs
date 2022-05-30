using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleManager : MonoBehaviour
{
    public Knight knight;

    public void NA_clicked()
    {
        knight.NormalAttack();
    }

    public void SA_clicked()
    {
        knight.SpecialAttack();
    }
}
