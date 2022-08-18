using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KWY {
    public class CustomTile : Tile
    {
        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            Debug.Log("·ÎÄÃÁÂÇ¥ : " + localTPos + ", ¿ùµåÁÂÇ¥ : " + worldTPos);
            base.RefreshTile(position, tilemap);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            //tileData.transform = transform;
            //tileData.gameObject = gameObject;
        }

        private int charCount;

        public Vector3Int localTPos;
        private Vector3 worldTPos;

        private List<GameObject> characters;


        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            localTPos = position;
            worldTPos = tilemap.GetComponent<Tilemap>().CellToWorld(position);
            //charCount = getInitialChar();
            charCount = 0;
            //Debug.Log("·ÎÄÃÁÂÇ¥ : " + localTPos + ", ¿ùµåÁÂÇ¥ : " + worldTPos);
            //Debug.Log("tileset");
            return base.StartUp(position, tilemap, go);
        }

        public int getInitialChar()
        {
            int count = 0;
            //if (characters != null)
            //    characters.Clear();

            characters = new List<GameObject>();

            Collider2D[] objects = Physics2D.OverlapCircleAll(worldTPos, 0.1f);
            //Collider[] objects = Physics.OverlapSphere(worldTPos, 0.1f);
            //if(objects.Length>0)
            //Debug.Log(objects.Length);

            if (objects.Length > 0)
            {
                foreach (Collider2D k in objects)
                {
                    if (k.gameObject.name != "Tilemap" && k.gameObject.name != "HighlightTilemap")
                    {
                        characters.Add(k.gameObject);
                        Debug.Log(k.gameObject.name);
                        count++;
                    }
                    //Debug.Log(k.gameObject.name);
                }
                //Debug.Log("Ä³¸¯ÅÍ ¼ö: " + count + ", ·ÎÄÃÁÂÇ¥ : " + localTPos + ", ¿ùµåÁÂÇ¥ : " + worldTPos);
                //Debug.Log(gameObject);
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


            return count;
        }

        public List<GameObject> getCharList()
        {
            return characters;
        }

        public int getCharCount()
        {
            return charCount;
        }

        public Vector3Int getTilePos()
        {
            Debug.Log("localpos = " + localTPos + ", worldpos = " + worldTPos);
            return localTPos;
        }

        public void updateCharNum(int num, GameObject ch)
        {
            charCount += num;
            if (num > 0)
            {
                characters.Add(ch);
                Debug.Log("added " + ch);
            }
            else
            {
                characters.Remove(ch);
                Debug.Log("deleted " + ch);
            }

            foreach(GameObject g in characters)
            {
                Debug.Log(g.name);
            }
            Debug.Log("at localPos: " + localTPos + ", worldPos: " + worldTPos);
            //this.gameObject.GetComponentInParent<Tilemap>().GetTransformMatrix(position);

            //this.transform.SetTRS(new Vector3(position.x, position.y + 0.2f, position.z), this.transform.rotation , new Vector3Int(2, 2, 2));
            //this.transform.MultiplyPoint(new Vector3Int(position.x, position.y + 1, position.z));

            //Debug.Log(this.transform);

            //this.transform.SetTRS(new Vector3Int(position.x, position.y + 1, position.z), this.transform.rotation , new Vector3Int(2, 2, 2));
            //charCount = getCharCount();
            //if (charCount > 1)
            //{
            //    this.transform.SetTRS(new Vector3Int(position.x, position.y + charCount, position.z), new Quaternion(0, 0, 0, 0), new Vector3Int(charCount, charCount, charCount));
            //}
            //else
            //    this.transform.SetTRS(new Vector3Int(position.x, position.y, position.z), new Quaternion(0, 0, 0, 0), new Vector3Int(1, 1, 1));
        }

        public void setCharPos()
        {
            int charCount = characters.Count;

            foreach(GameObject ch in characters){
                Vector3 originalPos = ch.transform.position;
                ch.transform.position.Set(originalPos.x, originalPos.y, originalPos.z);
            }
            
        }
        
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tiles/Custom Tile")]
        public static void CreateCustomTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Custom Tile", "New Custom Tile", "Asset", "Save Custom Tile", "Assets");
            if (path == "") return;

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CustomTile>(), path);
        }
#endif

    }
}
