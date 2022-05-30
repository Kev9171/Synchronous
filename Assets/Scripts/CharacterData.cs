using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Scriptable Object/Character Data", order = int.MaxValue)]

public class CharacterData : ScriptableObject
{
    [SerializeField]
    private string characterName;
    public string CharacterName { get { return characterName; } }

    [SerializeField]
    private int hp;
    public int HP { get { return hp; } }

    [SerializeField]
    private int mp;
    public int MP { get { return mp; } }

    [SerializeField]
    private int armor;
    public int Armor { get { return armor; } }

    [SerializeField]
    private int na_time;
    public int NA_TIME { get { return na_time; } }

    [SerializeField]
    private int sa_time;
    public int SA_TIME { get { return sa_time; } }
}
