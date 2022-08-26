using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Singletons/MasterManager")]
public class MasterManager : SingletonScriptableObject<MasterManager>
{
    [SerializeField]
    private GameSettings _gameSettings;

    public static GameSettings GameSettings
    {
        get
        {
            return Instance._gameSettings;
        }
    }

    // Test
    [SerializeField]
    private TestSettings _testSettings;

    public static TestSettings TestSettings
    {
        get
        {
            return Instance._testSettings;
        }
    }
}
