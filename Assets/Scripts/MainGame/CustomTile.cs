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
            base.RefreshTile(position, tilemap);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
        }

        private int charCount;

        private Vector3 worldTPos, localTPos;

        private List<GameObject> characters;

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            //Debug.Log(position);
            localTPos = position;
            worldTPos = tilemap.GetComponent<Tilemap>().CellToWorld(position);
            //tilePos = position;
            charCount = getCharCount();
            return base.StartUp(position, tilemap, go);
        }

        public int getCharCount()
        {
            int count = 0;
            if(characters != null)
                characters.Clear();


            Collider2D[] objects = Physics2D.OverlapCircleAll(worldTPos, 0.1f);//OverlapSphere(tilePos, 1f);
            if (objects.Length == 0 || objects == null)
            {
                // charCount = 0;
                //Debug.Log("캐릭터 없음");
            }
            else
            {
                foreach (Collider2D k in objects)
                {
                    if(k.gameObject.name != "Tilemap")
                    {
                        characters.Add(k.gameObject);
                        Debug.Log(k.gameObject.name);
                        count++;
                    }
                }
                Debug.Log("캐릭터 수: " + count + "좌표 : " + localTPos);
            }
            

            return count;
        }

        public void setTileSize(Vector3Int position)
        {
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
