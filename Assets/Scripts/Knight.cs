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
        Debug.Log("이름 : " + characterName);
        Debug.Log("HP : " + hp);
        Debug.Log("MP : " + mp);
        Debug.Log("피해감소율 : " + armor);
        Debug.Log("일반공격 시전시간 : " + na_time);
        Debug.Log("특수공격 시전시간 : " + sa_time);
        Debug.Log("------------------------------------------------------------");
    }

    void Start()
    {
        characterName = this.name;  //나중에 characterName.CharacterName으로 변경
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
        Debug.Log("패시브 발동중, "+ armor + "% 데미지 감소");
    }

    public void NormalAttack()
    {
        ResetTile();
        int damage = 10;
        int range_x = 1;
        int range_y = 0;
        Debug.Log("일반공격 시전, " + damage + "의 데미지");
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
        Debug.Log("특수공격 시전, 아군에게 " + armor + "% 데미지 감소 버프");
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
        Debug.Log(characterName+"이/가 "+ x + "의 데미지를 입어 체력이 "+ hp +"이/가 되었다.");
    }

    public void Buffed(String name, ref int target, int x)
    {
        target += x;
        Debug.Log(characterName + "의 " + name + "능력치가 " + x + "가 증가하여 "+ target + "이 되었다.");
    }
}
