using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Night : MonoBehaviour
{
    public CharacterData characterData;
    public Tilemap tilemap;

    private string characterName;
    private int hp;
    private int mp;
    private int armor;
    private int na_time;
    private int sa_time;

    public void PrintCharacterData()
    {
        Debug.Log("�̸� : " + characterName);
        Debug.Log("HP : " + hp);
        Debug.Log("MP : " + mp);
        Debug.Log("���ذ����� : " + armor);
        Debug.Log("�Ϲݰ��� �����ð� : " + na_time);
        Debug.Log("Ư������ �����ð� : " + sa_time);
        Debug.Log("------------------------------------------------------------");
    }

    void Start()
    {
        characterName = characterData.CharacterName;
        hp = characterData.HP;
        mp = characterData.MP;
        armor = characterData.Armor;
        na_time = characterData.NA_TIME;
        sa_time = characterData.SA_TIME;

        PrintCharacterData();
        Passive();
    }

    public void Passive()
    {
        armor += 20;
        Debug.Log("�нú� �ߵ���, "+ armor + "% ������ ����");
    }

    public int NormalAttack()
    {
        int damage = 10;
        Debug.Log("�Ϲݰ��� ����, " + damage + "�� ������");
        highlight(1, 0);
        return damage;
    }

    public void SpecialAttack()
    {
        Debug.Log("Ư������ ����, �Ʊ����� " + armor + "% ������ ���� ����");
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                highlight(i, j);
            }
        }
    }

    public void highlight(int x, int y)
    {
        Vector3 pos;
        pos = this.transform.position;

        Vector3Int range = tilemap.WorldToCell(pos);
        range.x += x;
        range.y += y;

        Debug.Log(range);

        this.tilemap.SetTileFlags(range, TileFlags.None);
        this.tilemap.SetColor(range, Color.red);
    }
}
