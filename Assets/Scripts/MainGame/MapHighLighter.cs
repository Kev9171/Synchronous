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
        private Vector3 lastPos;
        private List<Direction> allDirection = new List<Direction>();
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

        // raycast highlight
        public void HighlightMap(Vector3 basePos, SkillBase sb, bool reversed)
        {
            ClearHighlight();
            int dp;
            int rays = sb.directions.Count;
            List<Direction> dir = sb.directions;
            lastPos = basePos;

            for (int i = 0; i < rays; i++)
            {
                float d = sb.distance[i];

                if ((int)dir[i] == 6)
                {
                    lastPos = basePos;
                    continue;
                }
                else if (reversed)
                {
                    dp = (int)allDirection[5 - (int)dir[i]];
                }
                else
                {
                    dp = (int)dir[i];
                }

                for (int j = 1; j <= d; j++)
                {
                    switch (dp)
                    {
                        case 0:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + new Vector3(-0.5f, 0.5f) * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + new Vector3(-0.5f, 0.5f, 0) * j), highlightColor);
                            break;
                        case 1:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + Vector3.left * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + Vector3.left * j), highlightColor);
                            break;
                        case 2:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + new Vector3(-0.5f, -0.5f) * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + new Vector3(-0.5f, -0.5f) * j), highlightColor);
                            break;
                        case 3:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + new Vector3(0.5f, -0.5f) * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + new Vector3(0.5f, -0.5f) * j), highlightColor);
                            break;
                        case 4:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + Vector3.right * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + Vector3.right * j), highlightColor);
                            break;
                        case 5:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + new Vector3(0.5f, 0.5f) * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + new Vector3(0.5f, 0.5f) * j), highlightColor);
                            break;
                        case 6:
                            break;
                    }
                }
                switch (dp)
                {
                    case 0:
                        lastPos = lastPos + new Vector3(-0.5f, 0.5f) * d;
                        break;
                    case 1:
                        lastPos = lastPos + Vector3.left * d;
                        break;
                    case 2:
                        lastPos = lastPos + new Vector3(-0.5f, -0.5f) * d;
                        break;
                    case 3:
                        lastPos = lastPos + new Vector3(0.5f, -0.5f) * d;
                        break;
                    case 4:
                        lastPos = lastPos + Vector3.right * d;
                        break;
                    case 5:
                        lastPos = lastPos + new Vector3(0.5f, 0.5f) * d;
                        break;
                    case 6:
                        break;
                }
            }
        }

        // area highlight
        public void HighlightMap(Vector3 basePos, SkillBase sb)
        {
            ClearHighlight();
            for (int i = 0; i <= sb.distance[0]; i++)
            {
                if (i == 0)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos), highlightColor);
                    continue;
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(-0.5f, 0.5f) * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(-0.5f, 0.5f) * i), highlightColor);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(-0.5f, 0.5f) * i + Vector3.right * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(-0.5f, 0.5f) * i + Vector3.right * j), highlightColor);
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(0.5f, 0.5f) * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(0.5f, 0.5f) * i), highlightColor);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(0.5f, 0.5f) * i + new Vector3(0.5f, -0.5f) * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(0.5f, 0.5f) * i + new Vector3(0.5f, -0.5f) * j), highlightColor);
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + Vector3.right * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + Vector3.right * i), highlightColor);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + Vector3.right * i + new Vector3(-0.5f, -0.5f) * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + Vector3.right * i + new Vector3(-0.5f, -0.5f) * j), highlightColor);
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(0.5f, -0.5f) * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(0.5f, -0.5f) * i), highlightColor);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(0.5f, -0.5f) * i + Vector3.left * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(0.5f, -0.5f) * i + Vector3.left * j), highlightColor);
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(-0.5f, -0.5f) * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(-0.5f, -0.5f) * i), highlightColor);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(-0.5f, -0.5f) * i + new Vector3(-0.5f, 0.5f)  * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(-0.5f, -0.5f) * i + new Vector3(-0.5f, 0.5f) * j), highlightColor);
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + Vector3.left * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + Vector3.left * i), highlightColor);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + Vector3.left * i + new Vector3(0.5f, 0.5f) * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + Vector3.left * i + new Vector3(0.5f, 0.5f) * j), highlightColor);
                }
            }
        }

        public void ClearHighlight()
        {
            Vector3 pos = new Vector3(0, 0, 0);
            Vector3Int range;
            for (float i = -4; i < 5; i += 0.5f)
            {
                for (float j = -6; j < 6; j += 0.7f)
                {
                    pos.x = j;
                    pos.y = i;
                    range = hlMap.WorldToCell(pos);
                    this.hlMap.SetTileFlags(range, TileFlags.None);
                    this.hlMap.SetColor(range, transparent);
                }
            }
        }

        private void Start()
        {
            ClearHighlight();
            allDirection.Add(Direction.TopLeft);
            allDirection.Add(Direction.Left);
            allDirection.Add(Direction.BottomLeft);
            allDirection.Add(Direction.BottomRight);
            allDirection.Add(Direction.Right);
            allDirection.Add(Direction.TopRight);
            allDirection.Add(Direction.Base);
        }
    }
}

