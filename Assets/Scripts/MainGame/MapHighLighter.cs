using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;


namespace KWY
{
    public class MapHighLighter : MonoBehaviour
    {

        [SerializeField]
        Tilemap hlMap;

        [SerializeField]
        Color highlightColor;

        private Color transparent = new Color(1, 1, 1, 0);

        private void HighlightTile(Vector3 baseWorldPos, int x, int y)
        {
            hlMap.SetColor(hlMap.WorldToCell(baseWorldPos), highlightColor);
        }

        private List<Vector2Int> ReverseX(List<Vector2Int> posList)
        {
            List<Vector2Int> rv = new List<Vector2Int>();
            foreach(var v in posList)
            {
                rv.Add(new Vector2Int(v.x * -1, v.y));
            }

            return rv;
        }

        public void HighlightMapXReverse(Vector3Int baseTilePos, List<Vector2Int> posList)
        {
            ClearHighlight();

            // 임시로 하이라이트 방향은 기본 오른쪽
            // 마스터 클라이언트는 오른쪽으로 하이라이트,
            // 아니면 왼쪽으로 하이라이트
            foreach (Vector2Int pos in ReverseX(posList))
            {
                Vector3Int v = new Vector3Int(baseTilePos.x + pos.x, baseTilePos.y + pos.y, 0);
                if (hlMap.HasTile(v))
                {
                    hlMap.SetTileFlags(v, TileFlags.None);
                    hlMap.SetColor(v, highlightColor);
                }
            }
        }

        public void HighlightMap(Vector3Int baseTilePos, List<Vector2Int> posList)
        {
            ClearHighlight();
            
            foreach (Vector2Int pos in posList )
            {
                Vector3Int v = new Vector3Int(baseTilePos.x + pos.x, baseTilePos.y + pos.y, 0);
                if (hlMap.HasTile(v))
                {
                    hlMap.SetTileFlags(v, TileFlags.None);
                    hlMap.SetColor(v, highlightColor);
                }
            }
        }

        public void ChangeTileHeight(Vector3Int baseTilePos, Matrix4x4 height)
        {
            hlMap.SetTransformMatrix(baseTilePos, height);
        }

        public void ClearHighlight()
        {
            //Vector3 pos = new Vector3(0, 0, 0);
            //Vector3Int range;
            //for (float i = -4; i < 5; i += 0.5f)
            //{
            //    for (float j = -6; j < 6; j += 0.7f)
            //    {
            //        pos.x = j;
            //        pos.y = i;
            //        range = hlMap.WorldToCell(pos);
            //        this.hlMap.SetTileFlags(range, TileFlags.None);
            //        this.hlMap.SetColor(range, transparent);
            //    }
            //}

            foreach (var pos in hlMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                Vector3 place = hlMap.CellToWorld(localPlace);
                if (hlMap.HasTile(localPlace))
                {
                    hlMap.SetTileFlags(localPlace, TileFlags.None);
                    hlMap.SetColor(localPlace, transparent);
                }
            }

        }

        private void Start()
        {
            ClearHighlight();
        }
    }
}

