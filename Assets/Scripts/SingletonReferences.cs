using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonReferences : MonoBehaviour
{
    [SerializeField]
    private MasterManager _masterManager;

    [SerializeField]
    private KWY.CharaManager _characterManager;

    [SerializeField]
    private KWY.SkillManager _skillManager;

    [SerializeField]
    private KWY.BuffManager _buffManager;

    [SerializeField]
    private KWY.PlayerSkillManager playerSkillManager;
}
