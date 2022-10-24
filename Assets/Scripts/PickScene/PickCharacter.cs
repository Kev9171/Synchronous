using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Tilemaps;
using KWY;

namespace PickScene
{
    public class PickCharacter :MonoBehaviourPunCallbacks, IPunObservable 
    {
        [SerializeField] CharacterBase _characterBase;
        public CharacterBase Pcb { get; private set; }
        public Vector3Int TempTilePos { get; private set; }
        public Vector3Int SelTilePos { get; private set; }
        public Vector3 worldPos { get; private set; }
        [SerializeField] private float movementSpeed;
        private Vector2 destination;
        public Vector3Int TilePos;
        private Tilemap map, hlMap;
        private TilemapControl TCtrl;
        //private bool nowMove = false;

        private SkillSpawner skillSpawner;
        [SerializeField] private RayTest ray;
        public PickCharacter(CharacterBase cb)
        {
            Pcb = cb;
        }

        public PickCharacter(CharacterBase cb, Vector3Int pos)
        {
            Pcb = cb;
            TempTilePos = pos;
            Debug.Log("position: "+pos);
        }

        public void SetTilePos(Vector3Int pos)
        {
            TempTilePos = pos;
        }

        public void ResetTempPos()
        {
            map = GameObject.FindGameObjectWithTag("Map").GetComponent<Tilemap>();
            TempTilePos = TilePos;
        }

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// 이동으로 들어온 좌표를 현재 자신의 y 좌표에 맞게 실제 이동하게 되는 좌표로 바꿔주는 함수
        /// </summary>
        /// <param name="dir">dx, dy</param>
        /// <returns></returns>
        private Vector2Int TransFromY(Vector2Int dir)
        {
            // temp
            return dir;
        }

        #endregion

        #region MonoBehaviour CallBacks
        private void Awake()
        {
            Pcb = _characterBase;

            map = GameObject.FindGameObjectWithTag("Map").GetComponent<Tilemap>();
            //Debug.Log(map.name + " found.");
            hlMap = GameObject.Find("HighlightTilemap").GetComponent<Tilemap>();
            //Debug.Log(hlMap.name + " found.");
            TCtrl = GameObject.Find("TilemapControl").GetComponent<TilemapControl>();

            TilePos = map.WorldToCell(transform.position);
            //map.GetTile<CustomTile>(map.WorldToCell(transform.position)).updateCharNum(1, gameObject);
            //map.GetTile<CustomTile>(map.WorldToCell(transform.position)).getTilePos();

            Debug.Log(this+"'s pos = "+map.WorldToCell(transform.position));
        }
        #endregion


    }
}
