using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;


namespace KWY
{
    public class TilemapControl : MonoBehaviour
    {
        [SerializeField]
        GameObject[] tiles;
        [SerializeField]
        Sprite[] sprites;

        [System.Serializable]
        public class Coords
        {
            public List<Vector3> coordList;
        }
        [SerializeField]
        public List<Coords> nList = new List<Coords>();

        private int checkTileIdx(Vector3 worldPos)
        {
            //Matrix4x4 newMatrix = Matrix4x4.Scale(newTilepos);
            //tMap.SetTileFlags(curTilepos, TileFlags.None);
            //tMap.SetTransformMatrix(curTilepos, newMatrix);
            int count = 0;
            foreach(GameObject tile in tiles)
            {
                if (tile.transform.position == worldPos)
                    return count;
                count++;
            }
            return -1;
        }

        private int checkActiveTiles()
        {
            int count = 0;
            foreach (GameObject tile in tiles)
            {
                if (tile.activeSelf)
                    count++;
            }
            return count;
        }

        private int checkSpriteIdx(Sprite s)
        {
            int count = 0;
            foreach(Sprite tile in sprites)
            {
                if (tile == s)
                    return count;
                count++;
            }
            return -1;
        }

        public void activateTile(Vector3 worldPos, int charNum, Sprite sprite)
        {
            int num = checkActiveTiles();
            tiles[num].SetActive(true);
            tiles[num].transform.position = new Vector3(worldPos.x, worldPos.y + 0.1f, worldPos.z);
            tiles[num].transform.localScale = new Vector3(charNum, charNum, 1);
            tiles[num].GetComponent<SpriteRenderer>().sprite = sprites[checkSpriteIdx(sprite)];

            Tilemap hlMap = GameObject.Find("HighlightTilemap").GetComponent<Tilemap>();
            Matrix4x4 newPos = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), new Vector3(charNum - 0.6f, charNum - 0.6f, 1));
            hlMap.SetTransformMatrix(hlMap.WorldToCell(worldPos), newPos);

            //Debug.Log(new Vector3(worldPos.x, worldPos.y + 0.1f, worldPos.z));
        }

        public void deactivateTile(Vector3 worldPos)
        {
            Vector3 tilePos = new Vector3(worldPos.x, worldPos.y + 0.1f, worldPos.z);
            int num = checkTileIdx(tilePos);
            //tiles[num].transform.position.Set(worldPos.x, worldPos.y, worldPos.z);
            //tiles[num].GetComponent<SpriteRenderer>().sprite = sprites[checkSpriteIdx(sprite)];
            tiles[num].SetActive(false);

            Tilemap hlMap = GameObject.Find("HighlightTilemap").GetComponent<Tilemap>();
            Matrix4x4 newPos = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
            hlMap.SetTransformMatrix(hlMap.WorldToCell(worldPos), newPos);

        }

        //private void HighlightTile(Vector3 baseWorldPos, int x, int y)
        //{
        //    hlMap.SetColor(hlMap.WorldToCell(baseWorldPos), highlightColor);
        //}

        //private List<Vector2Int> ReverseX(List<Vector2Int> posList)
        //{
        //    List<Vector2Int> rv = new List<Vector2Int>();
        //    foreach (var v in posList)
        //    {
        //        rv.Add(new Vector2Int(v.x * -1, v.y));
        //    }

        //    return rv;
        //}

        //public void HighlightMapXReverse(Vector3Int baseTilePos, List<Vector2Int> posList)
        //{
        //    ClearHighlight();

        //    // 임시로 하이라이트 방향은 기본 오른쪽
        //    // 마스터 클라이언트는 오른쪽으로 하이라이트,
        //    // 아니면 왼쪽으로 하이라이트
        //    foreach (Vector2Int pos in ReverseX(posList))
        //    {
        //        Vector3Int v = new Vector3Int(baseTilePos.x + pos.x, baseTilePos.y + pos.y, 0);
        //        if (hlMap.HasTile(v))
        //        {
        //            hlMap.SetTileFlags(v, TileFlags.None);
        //            hlMap.SetColor(v, highlightColor);
        //        }
        //    }
        //}

        //public void HighlightMap(Vector3Int baseTilePos, List<Vector2Int> posList)
        //{
        //    ClearHighlight();

        //    foreach (Vector2Int pos in posList)
        //    {
        //        Vector3Int v = new Vector3Int(baseTilePos.x + pos.x, baseTilePos.y + pos.y, 0);
        //        if (hlMap.HasTile(v))
        //        {
        //            hlMap.SetTileFlags(v, TileFlags.None);
        //            hlMap.SetColor(v, highlightColor);
        //        }
        //    }
        //}

        //public void ClearHighlight()
        //{
        //    Vector3 pos = new Vector3(0, 0, 0);
        //    Vector3Int range;
        //    for (float i = -4; i < 5; i += 0.5f)
        //    {
        //        for (float j = -6; j < 6; j += 0.7f)
        //        {
        //            pos.x = j;
        //            pos.y = i;
        //            range = hlMap.WorldToCell(pos);
        //            this.hlMap.SetTileFlags(range, TileFlags.None);
        //            this.hlMap.SetColor(range, transparent);
        //        }
        //    }
        //}

        private void Start()
        {
            
        }
    }
}

