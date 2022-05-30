using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Knight : MonoBehaviour
{
    public CharacterData characterData;
    public Tilemap tilemap;

    private string characterName;
    private int hp;
    private int mp;
    private int armor;
    private int na_time;
    private int sa_time;
    private Color[,] tile_color = new Color[10, 12];

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
        ResetTile();
        int damage = 10;
        Debug.Log("�Ϲݰ��� ����, " + damage + "�� ������");
        AttackHighlight(1, 0);
        return damage;
    }

    public void SpecialAttack()
    {
        ResetTile();
        Debug.Log("Ư������ ����, �Ʊ����� " + armor + "% ������ ���� ����");
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                BuffHighlight(j, i);
            }
        }
    }

    public void AttackHighlight(int x, int y)
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

    public void BuffHighlight(int x, int y)
    {
        Vector3 pos;
        pos = this.transform.position;

        Vector3Int range = tilemap.WorldToCell(pos);
        range.x += x;
        range.y += y;

        Debug.Log(range);

        this.tilemap.SetTileFlags(range, TileFlags.None);
        this.tilemap.SetColor(range, Color.blue);
    }

    public void ResetTile()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        Vector3Int range;
        for (float i = -4; i < 5; i += 0.5f)
        {
            for (float j = -6; j < 6; j += 0.9f)
            {
                pos.x = j;
                pos.y = i;
                range = tilemap.WorldToCell(pos);
                this.tilemap.SetTileFlags(range, TileFlags.None);
                this.tilemap.SetColor(range, Color.white);
            }
        }
    }
}
