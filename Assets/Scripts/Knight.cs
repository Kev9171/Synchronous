using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Knight : MonoBehaviour
{
    public CharacterData characterData;
    public Tilemap tilemap;
    public Tilemap HL_tilemap;
    public Knight opponent;

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
        characterName = this.name;  //���߿� characterName.CharacterName���� ����
        hp = characterData.HP;
        mp = characterData.MP;
        armor = characterData.Armor;
        na_time = characterData.NA_TIME;
        sa_time = characterData.SA_TIME;

        ResetTile();
        PrintCharacterData();
        HL_tilemap.gameObject.SetActive(true);
        Passive();
    }

    public void Passive()
    {
        armor += 20;
        Debug.Log("�нú� �ߵ���, "+ armor + "% ������ ����");
    }

    public void NormalAttack()
    {
        ResetTile();
        int damage = 10;
        int range_x = 1;
        int range_y = 0;
        Debug.Log("�Ϲݰ��� ����, " + damage + "�� ������");
        AttackHighlight(range_x, range_y);
        if(Mathf.Abs(this.transform.position.x - opponent.transform.position.x) <= range_x && 
            Mathf.Abs(this.transform.position.y - opponent.transform.position.y) <= range_y)
        {
            opponent.Damaged(damage);
        }
    }

    public void SpecialAttack()
    {
        ResetTile();
        int range_x = 2;
        int range_y = 2;
        Debug.Log("Ư������ ����, �Ʊ����� " + armor + "% ������ ���� ����");
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                BuffHighlight(j, i);
            }
        }
        if (Mathf.Abs(this.transform.position.x - opponent.transform.position.x) <= range_x &&
            Mathf.Abs(this.transform.position.y - opponent.transform.position.y) <= range_y)
        {
            opponent.Buffed("armor", ref armor, armor);
        }
    }

    public void AttackHighlight(int x, int y)
    {
        Vector3 pos;
        Color color = new Color(1, 1, 1, 1);
        pos = this.transform.position;

        Vector3Int range = HL_tilemap.WorldToCell(pos);
        range.x += x;
        range.y += y;

        Debug.Log(range);

        this.HL_tilemap.SetTileFlags(range, TileFlags.None);
        this.HL_tilemap.SetColor(range, color);
    }

    public void BuffHighlight(int x, int y)
    {
        Vector3 pos;
        Color color = new Color(1, 1, 1, 1);
        pos = this.transform.position;

        Vector3Int range = HL_tilemap.WorldToCell(pos);
        range.x += x;
        range.y += y;

        Debug.Log(range);

        this.HL_tilemap.SetTileFlags(range, TileFlags.None);
        this.HL_tilemap.SetColor(range, color);
    }

    public void ResetTile()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        Color color = new Color(1, 1, 1, 0);
        Vector3Int range;
        for (float i = -4; i < 5; i += 0.5f)
        {
            for (float j = -6; j < 6; j += 0.7f)
            {
                pos.x = j;
                pos.y = i;
                range = HL_tilemap.WorldToCell(pos);
                this.HL_tilemap.SetTileFlags(range, TileFlags.None);
                this.HL_tilemap.SetColor(range, color);
            }
        }
    }

    public void Damaged(int x)
    {
        hp -= x;
        Debug.Log(characterName+"��/�� "+ x + "�� �������� �Ծ� ü���� "+ hp +"��/�� �Ǿ���.");
    }

    public void Buffed(String name, ref int target, int x)
    {
        target += x;
        Debug.Log(characterName + "�� " + name + "�ɷ�ġ�� " + x + "�� �����Ͽ� "+ target + "�� �Ǿ���.");
    }
}
