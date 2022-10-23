using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;
using KWY;


namespace KWY
{
    public class TilemapControl : MonoBehaviour
    {
        [SerializeField]
        Tilemap map;
        [SerializeField]
        GameObject[] tiles;
        [SerializeField]
        Sprite[] sprites;

        private Dictionary<Vector3Int, List<GameObject>> Characters = new Dictionary<Vector3Int, List<GameObject>>();

        [System.Serializable]
        public class Coords
        {
            public List<Vector3> coordList;
        }
        [SerializeField]
        public List<Coords> nList = new List<Coords>();

        private int checkAltTileIdx(Vector3 worldPos)
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

        private int checkActiveAltTiles()
        {
            int count = 0;
            foreach (GameObject tile in tiles)
            {
                if (tile.GetComponent<SpriteRenderer>().color == Color.white)
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

        public void activateAltTile(Vector3 worldPos, int charNum, Sprite sprite)
        {
            int num = checkActiveAltTiles();
            if (num < 3)
            {
                Vector3 pos = new Vector3(worldPos.x, worldPos.y + 0.1f, worldPos.z);
                int idx = checkAltTileIdx(pos);
                if(idx == -1)
                {
                    tiles[num].GetComponent<SpriteRenderer>().color = Color.white;
                    tiles[num].transform.position = pos;
                    tiles[num].transform.localScale = new Vector3(charNum, charNum, 1);
                    tiles[num].GetComponent<SpriteRenderer>().sprite = sprites[checkSpriteIdx(sprite)];
                }
                else
                {
                    tiles[idx].transform.localScale = new Vector3(charNum, charNum, 1);
                }
                
            }
            else
            {
                Debug.Log("over 3 tiles active");
            }

            Tilemap hlMap = GameObject.Find("HighlightTilemap").GetComponent<Tilemap>();
            Matrix4x4 newPos = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), new Vector3(charNum - 0.6f, charNum - 0.6f, 1));
            hlMap.SetTransformMatrix(hlMap.WorldToCell(worldPos), newPos);

            //Debug.Log(new Vector3(worldPos.x, worldPos.y + 0.1f, worldPos.z));
        }

        public void deactivateAltTile(Vector3 worldPos)
        {
            Vector3 tilePos = new Vector3(worldPos.x, worldPos.y + 0.2f, worldPos.z);
            int idx = checkAltTileIdx(tilePos);
            //tiles[num].transform.position.Set(worldPos.x, worldPos.y, worldPos.z);
            //tiles[num].GetComponent<SpriteRenderer>().sprite = sprites[checkSpriteIdx(sprite)];
            if(idx != -1)
            {
                tiles[idx].transform.position = Vector3.zero;
                tiles[idx].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                Debug.Log("alt tile deleted");
            }
            else
                Debug.Log("no tile found");

            Tilemap hlMap = GameObject.Find("HighlightTilemap").GetComponent<Tilemap>();
            Matrix4x4 newPos = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
            hlMap.SetTransformMatrix(hlMap.WorldToCell(worldPos), newPos);

        }
        public interface ILifecycleTile
        {
            void TileAwake(Vector3Int position, Tilemap tilemap);
            void TileStart(Vector3Int position, Tilemap tilemap);
        }

        //private void Awake()
        //{
        //    map = GetComponent<Tilemap>();
        //    if (map == null)
        //    {
        //        throw ProgramUtils.MissingComponentException(typeof(Tilemap));
        //    }
        //    foreach (Vector3Int position in map.cellBounds.allPositionsWithin)
        //    {
        //        if (map.GetTile(position) is ILifecycleTile tile)
        //        {
        //            tile.TileAwake(position, map);
        //        }
        //    }
        //}
        private void Start()
        {
            SetTiles();
            //Invoke("SetTiles", 2f);
            
        }

        private void SetTiles()
        {
            foreach (Vector3Int position in map.cellBounds.allPositionsWithin)
            {
                if (map.HasTile(position))
                {
                    List<GameObject> ch = new List<GameObject>();

                    ch = getInitialChar(map.CellToWorld(position));
                    if (ch.Count > 0)
                        Debug.Log("at position: " + position);
                    Characters.Add(position, ch);
                    
                    //CustomTile tile = map.GetTile(position) as CustomTile;
                    //tile.getTilePos();
                    //Debug.Log("normal/pos = "+position+", tilename = "+map.GetTile(position).name);
                    //Debug.Log("custom/pos = " + position + ", tilename = " + map.GetTile<CustomTile>(position).name);
                    //map.GetTile<CustomTile>(position).getTilePos();
                    //Debug.Log("pos = " + position + ", tile = " + map.GetTile<CustomTile>(position).getTilePos());
                }
            }
        }
        public List<GameObject> getInitialChar(Vector3 position)
        {
            int count = 0;
            //if (characters != null)
            //    characters.Clear();

            List<GameObject> ch = new List<GameObject>();

            Collider2D[] objects = Physics2D.OverlapCircleAll(position, 0.1f);
            //Collider[] objects = Physics.OverlapSphere(worldTPos, 0.1f);
            //if(objects.Length>0)
            //Debug.Log(objects.Length);

            if (objects.Length > 0)
            {
                foreach (Collider2D k in objects)
                {
                    if (k.gameObject.tag == "Friendly" || k.gameObject.tag == "Enemy")
                    {
                        ch.Add(k.gameObject);
                        Debug.Log(k.gameObject.name);
                        count++;
                    }
                    //Debug.Log(k.gameObject.name);
                }
                Debug.Log("캐릭터 수: " + count);
                if (count > 0)
                {
                    //Debug.Log(gameObject.name);
                    //gameObject.SetActive(false);
                    //DestroyImmediate(gameObject, true);
                }
            }
            else
            {

            }

            return ch;
        }

        public void updateCharNum(Vector3Int pos, int num, GameObject ch)
        {
            //charCount += num;
            if (num > 0)
            {
                Characters.TryGetValue(pos, out List < GameObject > charnum);
                charnum.Add(ch);
                Characters[pos] = charnum;
                Debug.Log("added " + ch);
            }
            else
            {
                //Characters.
                Characters.TryGetValue(pos, out List<GameObject> charnum);
                if (charnum.Contains(ch))
                {
                    charnum.Remove(ch);
                    Characters[pos] = charnum;
                    Debug.Log("deleted " + ch);
                }
                else
                    Debug.Log(ch + " not found");
            }
        }

        public List<GameObject> getCharList(Vector3Int pos)
        {
            return Characters[pos];
        }
    }
}

