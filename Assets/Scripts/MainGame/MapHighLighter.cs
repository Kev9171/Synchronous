using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


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

        public void HighlightMap(Vector3 baseWorldPos, List<Vector2Int> posList)
        {
            ClearHighlight();

            Vector3Int baseV = hlMap.WorldToCell(baseWorldPos);
            foreach (Vector2Int pos in posList)
            {
                Vector3Int v = new Vector3Int(baseV.x + pos.x, baseV.y + pos.y, 0);
                if (hlMap.HasTile(v))
                {
                    hlMap.SetTileFlags(v, TileFlags.None);
                    hlMap.SetColor(v, highlightColor);
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
        }
    }
}

