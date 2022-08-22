using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KWY
{
    public class Character :MonoBehaviourPunCallbacks, IPunObservable 
    {
        [SerializeField]
        CharacterBase _characterBase;

        public CharacterBase Cb { get; private set; }
        public List<Buff> Buffs { get; private set; }
        public float Hp { get; private set; }
        public float Mp { get; private set; }
        public bool BreakDown { get; private set; }
        public Vector3Int TempTilePos { get; private set; }

        public Vector3 worldPos { get; private set; }

        public static readonly float MaxMp = 10;

        [SerializeField] private float movementSpeed;
        private Vector2 destination;
        public Vector3Int TilePos;

        private Tilemap map, hlMap;
        private TilemapControl TCtrl;

        private bool nowMove = false;

        public Character(CharacterBase cb)
        {
            Cb = cb;
            Buffs = new List<Buff>();
            Hp = cb.hp;
            Mp = 0;
            BreakDown = false;
        }

        public Character(CharacterBase cb, Vector3Int pos)
        {
            Cb = cb;
            Buffs = new List<Buff>();
            Hp = cb.hp;
            Mp = 0;
            BreakDown = false;
            TempTilePos = pos;
            Debug.Log("position: "+pos);
        }

        public void DamageHP(float damage)
        {
            Hp -= damage;
            if (Hp < 0) Hp = 0;

            if (Hp == 0)
            {
                BreakDown = true;
                ClearBuff();
                Debug.LogFormat("{0} is damaged {1}; Now hp: {2}; BREAK DOWN!", Cb.name, damage, Hp);
            }
            else
            {
                Debug.LogFormat("{0} is damaged {1}; Now hp: {2}", Cb.name, damage, Hp);
            }
        }

        public void AddMP(float amount)
        {
            Mp += amount;
            if (Mp > MaxMp) Mp = MaxMp;

            Debug.LogFormat("{0}'s mp is added {1}; Now mp: {2}", Cb.name, amount, Mp);
        }

        public void AddBuff(BuffBase bb, int turn)
        {
            Buffs.Add(new Buff(bb, turn));
        }

        public void ReduceBuffTurn(int turn)
        {
            foreach(Buff b in Buffs)
            {
                b.turn -= turn;
                if (b.turn <= 0)
                {
                    Buffs.Remove(b);
                    Debug.LogFormat("The buff {0} of {1} is removed", b.bb.name, Cb.name);
                }
            }
        }

        public void ClearBuff()
        {
            Buffs.Clear();
            Debug.LogFormat("All buffs of {0} is removed", Cb.name);
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

        public override string ToString()
        {
            string t = "[";
            foreach(Buff b in Buffs)
            {
                t += b.ToString() + ", ";
            }
            t += "]";
            return string.Format("CID: {0}, HP: {1}, MP: {2}, Down?: {3}, Buffs: {4}", Cb.cid, Hp, Mp, BreakDown, t);
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


        #region Simulation Functions

        [PunRPC]
        public void MoveTo(int x, int y)
        {
            Vector2Int dir = new Vector2Int(x, y);

            Vector2Int realDir = TransFromY(dir);
            Vector3Int nowPos = TilePos;
            Vector3 des = map.CellToWorld(new Vector3Int(nowPos.x + realDir.x, nowPos.y + realDir.y, nowPos.z));
            TilePos = map.WorldToCell(des);
            des.y += 0.1f;

            if (map.HasTile(map.WorldToCell(des)))
            {
                TCtrl.updateCharNum(map.WorldToCell(des), 1, gameObject);
                TCtrl.updateCharNum(nowPos, -1, gameObject);

                int charsOnDes = TCtrl.getCharList(map.WorldToCell(des)).Count;
                int charsOnCur = TCtrl.getCharList(nowPos).Count;

                worldPos = des;
                destination = des;
                //nowMove = true;
                Debug.Log("nowpos = " + nowPos + ", despos = " + map.WorldToCell(des));
                Debug.Log(gameObject.name + ": desNum->" + charsOnDes + ", curNum->" + charsOnCur);

                if (map.CellToWorld(nowPos).x < des.x)
                    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                else if(map.CellToWorld(nowPos).x > des.x)
                    gameObject.GetComponent<SpriteRenderer>().flipX = true;

                if (charsOnDes > 1)
                {
                    //map.SetTransformMatrix(map.WorldToCell(des), elevatedTile);
                    //hlMap.SetTransformMatrix(map.WorldToCell(des), elevatedTile);
                    map.SetTileFlags(map.WorldToCell(des), TileFlags.None);
                    map.SetColor(map.WorldToCell(des), new Color(1, 1, 1, 0));

                    Sprite sprite = map.GetTile<CustomTile>(map.WorldToCell(des)).sprite;
                    TCtrl.activateAltTile(des, charsOnDes, sprite);

                    List<GameObject> characters = TCtrl.getCharList(map.WorldToCell(des));

                    int count = 0;

                    //nowMove = true;
                    foreach (GameObject chara in characters)
                    {
                        //chara.GetComponent<Character>().worldPos = des;
                        Vector2 offset = TCtrl.nList[charsOnDes - 1].coordList[count];
                        chara.GetComponent<Character>().destination = (Vector2)chara.GetComponent<Character>().worldPos + offset;
                        chara.GetComponent<Character>().nowMove = true;
                        //chara.transform.position += new Vector3(-0.1f, 0.5f, 0);
                        chara.GetComponent<BoxCollider2D>().offset = -offset;

                        //Debug.Log(chara.GetComponent<Character>().destination);
                        //Debug.Log((Vector2)fTiles.nList[charsOnDes - 1].coordList[count] + ", " + (Vector2)fTiles.nList[charsOnDes - 2].coordList[count]);

                        count++;
                    }
                }
                else
                {
                    nowMove = true;
                    gameObject.GetComponent<BoxCollider2D>().offset = Vector2.zero;

                    Debug.Log("noone on tile");
                }

                if (charsOnCur > 1)
                {
                    Sprite sprite = map.GetTile<CustomTile>(nowPos).sprite;
                    Vector3 vec = map.CellToWorld(nowPos);
                    vec.y += 0.1f;
                    TCtrl.activateAltTile(vec, charsOnCur, sprite);

                    List<GameObject> characters = TCtrl.getCharList(nowPos);

                    int count = 0;
                    destination = des;
                    foreach (GameObject chara in characters)
                    {
                        Vector3 charpos = chara.transform.position;
                        Vector2 offset = TCtrl.nList[charsOnCur - 1].coordList[count];
                        chara.GetComponent<Character>().destination = (Vector2)chara.GetComponent<Character>().worldPos + offset;
                        chara.GetComponent<Character>().nowMove = true;
                        //chara.transform.position += new Vector3(-0.1f, 0.5f, 0);
                        chara.GetComponent<BoxCollider2D>().offset = -offset;

                        count++;
                    }
                }

                else if (charsOnCur == 1)
                {
                    //map.SetTransformMatrix(map.WorldToCell(des), groundTile);
                    //hlMap.SetTransformMatrix(map.WorldToCell(des), groundTile);

                    map.SetTileFlags(nowPos, TileFlags.None);
                    map.SetColor(nowPos, new Color(1, 1, 1, 1));

                    TCtrl.deactivateAltTile(map.CellToWorld(nowPos));

                    List<GameObject> characters = TCtrl.getCharList(nowPos);
                    characters[0].GetComponent<Character>().destination = map.CellToWorld(nowPos);
                    characters[0].GetComponent<Character>().nowMove = true;
                    characters[0].GetComponent<BoxCollider2D>().offset = Vector2.zero;
                    GetComponent<BoxCollider2D>().offset = Vector2.zero;

                }

                Debug.LogFormat("{0} / {1} is moving to {2}", PhotonNetwork.IsMasterClient ? 'M' : 'C', Cb.cid, map.WorldToCell(des));
            }

            //if (map.HasTile(map.WorldToCell(des)))
            //{
            //    destination = des;
            //    nowMove = true;

            //    //Matrix4x4 groundTile = Matrix4x4.TRS(new Vector3(0, 0f, 0), Quaternion.Euler(0f, 0f, 0f), Vector3.one);
            //    //Matrix4x4 elevatedTile = Matrix4x4.TRS(new Vector3(0, 0.2f, 0), Quaternion.Euler(0f, 0f, 0f), Vector3.one/*scale 조정*/);

            //    Debug.LogFormat("{0} / {1} is moving to {2}", PhotonNetwork.IsMasterClient ? 'M' : 'C', Cb.cid, map.WorldToCell(des));
            //}
            else
            {
                Debug.LogFormat("{0} / {1} can not go to {2}", PhotonNetwork.IsMasterClient ? 'M' : 'C', Cb.cid, map.WorldToCell(des));
            }
        }

        public void SpellSkill(SID sid, SkillDicection direction)
        {
            nowMove = false;
            Debug.LogFormat("{0} / {1} spells {2}", PhotonNetwork.IsMasterClient ? 'M' : 'C', Cb.cid, sid);
        }

        #endregion
        

        #region MonoBehaviour CallBacks
        private void Awake()
        {
            Cb = _characterBase;
            Buffs = new List<Buff>();

            map = GameObject.FindGameObjectWithTag("Map").GetComponent<Tilemap>();
            hlMap = GameObject.Find("HighlightTilemap").GetComponent<Tilemap>();
            TCtrl = GameObject.Find("TilemapControl").GetComponent<TilemapControl>();

            TilePos = map.WorldToCell(transform.position);
            //map.GetTile<CustomTile>(map.WorldToCell(transform.position)).updateCharNum(1, gameObject);
            //map.GetTile<CustomTile>(map.WorldToCell(transform.position)).getTilePos();

            Debug.Log(this+"'s pos = "+map.WorldToCell(transform.position));
        }

        void Update()
        {
            if (nowMove)
            {
                transform.position = Vector3.Lerp(gameObject.transform.position, destination, 0.7f);
                if (transform.position == new Vector3(destination.x, destination.y, 0))
                    nowMove = false;
            }
            else
            {
                
            }
        }
        #endregion


    }
}
