using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;


namespace KWY
{
    public class MapHighLighter : MonoBehaviourPunCallbacks
    {

        [SerializeField]
        Tilemap hlMap;

        [SerializeField]
        Color highlightColor;

        private Dictionary<CID, List<Vector3Int>> Highlights = new Dictionary<CID, List<Vector3Int>>();

        private Color transparent = new Color(1, 1, 1, 0);
        private Vector3 lastPos;
        private List<Direction> allDirection = new List<Direction>();
        private float xCor = 0.65f * 1.5f;  //맵1 -> 맵2 x값 보정치
        private float yCor = 0.7f / 0.65f * 1.5f;  // 맵1 -> 맵2 y값 보정치
        private void HighlightTile(Vector3 baseWorldPos, int x, int y)
        {
            hlMap.SetColor(hlMap.WorldToCell(baseWorldPos), highlightColor);
        }

        private List<Vector2Int> ReverseX(List<Vector2Int> posList)
        {
            List<Vector2Int> rv = new List<Vector2Int>();
            foreach (var v in posList)
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
            foreach (Vector2Int pos in posList)
            {
                Vector3Int v = new Vector3Int(baseTilePos.x + pos.x, baseTilePos.y + pos.y, 0);
                if (hlMap.HasTile(v))
                {
                    hlMap.SetTileFlags(v, TileFlags.None);
                    hlMap.SetColor(v, highlightColor);
                }
            }
        }

        [PunRPC]
        public void PhotonHighlightMap(Vector3 baseTilePos, bool? c = null)
        {
            ClearHighlight();

            Color color;
            if (c == null) color = highlightColor;
            else color = Color.red;

            List<Vector2Int> posList = baseTilePos.y % 2 == 0 ? MoveManager.MoveData.areaEvenY : MoveManager.MoveData.areaOddY;
            posList.Add(Vector2Int.zero);
            foreach (Vector2 pos in posList)
            {
                Vector3Int v = new Vector3Int((int)baseTilePos.x + (int)pos.x, (int)baseTilePos.y + (int)pos.y, 0);
                if (hlMap.HasTile(v))
                {
                    hlMap.SetTileFlags(v, TileFlags.None);
                    hlMap.SetColor(v, (Color)color);
                }
            }
        }

        // raycast highlight
        [PunRPC]
        public void HighlightMap(Vector3 basePos, int sid/*SkillBase sb*/, int overdir, bool? c = null)
        {
            SkillBase sb = SkillManager.GetData((SID)sid);
            Color color;
            if (c == null) color = highlightColor;
            else color = Color.red;
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
                //else if (reversed)
                //{
                //    dp = (int)allDirection[5 - (int)dir[i]];
                //}
                //else
                //{
                //    dp = (int)dir[i];
                //}

                if (overdir == 0)
                {
                    dp = (int)allDirection[((int)dir[i] + 5) % 6];
                }
                else if (overdir == 1)
                {
                    dp = (int)dir[i];
                }
                else if (overdir == 2)
                {
                    dp = (int)allDirection[((int)dir[i] + 1) % 6];
                }
                else if (overdir == 3)
                {
                    dp = (int)allDirection[((int)dir[i] + 2) % 6];
                }
                else if (overdir == 4)
                {
                    dp = (int)allDirection[((int)dir[i] + 3) % 6];
                }
                else
                {
                    dp = (int)allDirection[((int)dir[i] + 4) % 6];
                }

                for (int j = 1; j <= d; j++)
                {
                    switch (dp)
                    {
                        case 0:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + new Vector3(-0.5f * xCor, 0.5f * yCor) * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + new Vector3(-0.5f * xCor, 0.5f * yCor) * j), (Color)color);
                            break;
                        case 1:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + Vector3.left * xCor * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + Vector3.left * xCor * j), (Color)color);
                            break;
                        case 2:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + new Vector3(-0.5f * xCor, -0.5f * yCor) * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + new Vector3(-0.5f * xCor, -0.5f * yCor) * j), (Color)color);
                            break;
                        case 3:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + new Vector3(0.5f * xCor, -0.5f * yCor) * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + new Vector3(0.5f * xCor, -0.5f * yCor) * j), (Color)color);
                            break;
                        case 4:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + Vector3.right * xCor * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + Vector3.right * xCor * j), (Color)color);
                            break;
                        case 5:
                            hlMap.SetTileFlags(hlMap.WorldToCell(lastPos + new Vector3(0.5f * xCor, 0.5f * yCor) * j), TileFlags.None);
                            hlMap.SetColor(hlMap.WorldToCell(lastPos + new Vector3(0.5f * xCor, 0.5f * yCor) * j), (Color)color);
                            break;
                        case 6:
                            break;
                    }
                }
                switch (dp)
                {
                    case 0:
                        lastPos = lastPos + new Vector3(-0.5f * xCor, 0.5f * yCor) * d;
                        break;
                    case 1:
                        lastPos = lastPos + Vector3.left * xCor * d;
                        break;
                    case 2:
                        lastPos = lastPos + new Vector3(-0.5f * xCor, -0.5f * yCor) * d;
                        break;
                    case 3:
                        lastPos = lastPos + new Vector3(0.5f * xCor, -0.5f * yCor) * d;
                        break;
                    case 4:
                        lastPos = lastPos + Vector3.right * xCor * d;
                        break;
                    case 5:
                        lastPos = lastPos + new Vector3(0.5f * xCor, 0.5f * yCor) * d;
                        break;
                    case 6:
                        break;
                }
            }
        }

        // area highlight
        [PunRPC]
        public void HighlightMap(Vector3 basePos, int sid/*SkillBase sb*/, bool? c = null)
        {
            SkillBase sb = SkillManager.GetData((SID)sid);
            Color color;
            if (c == null) color = highlightColor;
            else color = Color.red;
            ClearHighlight();
            for (int i = 0; i <= sb.distance[0]; i++)
            {
                if (i == 0)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos), (Color)color);
                    continue;
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(-0.5f * xCor, 0.5f * yCor) * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(-0.5f * xCor, 0.5f * yCor) * i), (Color)color);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(-0.5f * xCor, 0.5f * yCor) * i + Vector3.right * xCor * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(-0.5f * xCor, 0.5f * yCor) * i + Vector3.right * xCor * j), (Color)color);
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(0.5f * xCor, 0.5f * yCor) * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(0.5f * xCor, 0.5f * yCor) * i), (Color)color);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(0.5f * xCor, 0.5f * yCor) * i + new Vector3(0.5f * xCor, -0.5f * yCor) * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(0.5f * xCor, 0.5f * yCor) * i + new Vector3(0.5f * xCor, -0.5f * yCor) * j), (Color)color);
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + Vector3.right * xCor * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + Vector3.right * xCor * i), (Color)color);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + Vector3.right * xCor * i + new Vector3(-0.5f * xCor, -0.5f * yCor) * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + Vector3.right * xCor * i + new Vector3(-0.5f * xCor, -0.5f * yCor) * j), (Color)color);
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(0.5f * xCor, -0.5f * yCor) * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(0.5f * xCor, -0.5f * yCor) * i), (Color)color);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(0.5f * xCor, -0.5f * yCor) * i + Vector3.left * xCor * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(0.5f * xCor, -0.5f * yCor) * i + Vector3.left * xCor * j), (Color)color);
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(-0.5f * xCor, -0.5f * yCor) * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(-0.5f * xCor, -0.5f * yCor) * i), (Color)color);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + new Vector3(-0.5f * xCor, -0.5f * yCor) * i + new Vector3(-0.5f * xCor, 0.5f * yCor) * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + new Vector3(-0.5f * xCor, -0.5f * yCor) * i + new Vector3(-0.5f * xCor, 0.5f * yCor) * j), (Color)color);
                }

                hlMap.SetTileFlags(hlMap.WorldToCell(basePos + Vector3.left * xCor * i), TileFlags.None);
                hlMap.SetColor(hlMap.WorldToCell(basePos + Vector3.left * xCor * i), (Color)color);

                for (int j = 1; j <= i - 1; j++)
                {
                    hlMap.SetTileFlags(hlMap.WorldToCell(basePos + Vector3.left * xCor * i + new Vector3(0.5f * xCor, 0.5f * yCor) * j), TileFlags.None);
                    hlMap.SetColor(hlMap.WorldToCell(basePos + Vector3.left * xCor * i + new Vector3(0.5f * xCor, 0.5f * yCor) * j), (Color)color);
                }
            }
        }

        public void ClearHighlight()
        {
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
        [PunRPC]
        public void PhotonClearHighlight()
        {
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

