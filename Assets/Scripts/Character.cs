using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Tilemaps;

using DebugUtil;

namespace KWY
{
    public class Character : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable, ISubject<Character>
    {
        [SerializeField]
        CharacterBase _characterBase;

        private readonly List<IObserver<Character>> observers = new List<IObserver<Character>>();

        public PlayableCharacter Pc
        {
            get;
            private set;
        } = null;

        public List<IObserver<Character>> Observers
        {
            get;
            private set;
        } = new List<IObserver<Character>>();

        public List<Buff> Buffs
        {
            get;
            private set;
        } = new List<Buff>();
        public CharacterBase Cb { get; private set; }
        public int Hp { get; private set; }
        public int Mp { get; private set; }
        public int MaxHp { get; private set; }
        public int MaxMp { get; private set; }
        public bool BreakDown { get; private set; }
        public int Atk { get; private set; }
        public int Def { get; private set; }

        public int TempMp { get; set; }


        private SkillSpawner skillSpawner;

        private RayTest ray;


        public Vector3Int TempTilePos { get; private set; }

        public Vector3 worldPos { get; private set; }

        private readonly float movementSpeed = 0.5f;
        private Vector2 destination;
        public Vector3Int TilePos;
        public int moveIdx { get; private set; }

        private Tilemap map;
        private TilemapControl TCtrl;

        private bool nowMove = false;

        public void SetData(PlayableCharacter pc)
        {
            Pc = pc;
        }

        private void Init()
        {
            Cb = _characterBase;

            Hp = Cb.hp;
            Mp = 0; // TODO

            BreakDown = false;

            MaxHp = Cb.hp;
            MaxMp = 10;

            Atk = Cb.atk;
            Def = 1; // TODO

            TempMp = Mp;
        }

        public void DamageHP(int damage)
        {
            if (Hp - damage > 0)
            {
                Hp -= damage;
                Debug.LogFormat("{0} is damaged {1}; Now hp: {2}", Cb.name, damage, Hp);
            }
            else if (Hp - damage < 0)
            {
                Hp = 0;
                BreakDown = true;
                GetComponent<SpriteRenderer>().color = Color.red;
                ClearBuff();
                Debug.LogFormat("{0} is damaged {1}; Now hp: {2}; BREAK DOWN!", Cb.name, damage, Hp);
            }

            NotifyObservers();
        }

        public void AddMP(int amount)
        {
            if (Mp + amount > MaxMp)
            {
                Mp = MaxMp;
            }
            else if (Mp + amount < 0)
            {
                Mp = 0;
            }
            else
            {
                Mp += amount;
            }

            TempMp = Mp;

            Debug.LogFormat($"[id={Pc.Id}]{Cb.name}'s mp is added {amount}; Now mp: {Mp}");

            NotifyObservers();
        }

        public void AddHp(int amount)
        {
            if (Hp + amount > MaxMp)
            {
                Hp = MaxHp;
            }
            else if (Hp - amount < 0)
            {
                Hp = 0;
            }
            else
            {
                Hp += amount;
            }

            Debug.LogFormat("{0}'s hp is added {1}; Now hp: {2}", Cb.name, amount, Hp);

            NotifyObservers();
        }

        public void AddBuff(BuffBase bb, int turn)
        {
            Buffs.Add(new Buff(bb, turn));

            NotifyObservers();
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

            NotifyObservers();
        }

        public void ClearBuff()
        {
            Buffs.Clear();
            NotifyObservers();
        }

        public void SetTilePos(Vector3Int pos)
        {
            TempTilePos = pos;
        }

        public void ResetTempPosAndMp()
        {
            TempTilePos = map.WorldToCell(transform.position);
            TempMp = Mp;
        }

        public override string ToString()
        {
            string t = "[";
            foreach(Buff b in Buffs)
            {
                t += b.ToString() + ", ";
            }
            t += "]";

            string tt = "[";
            foreach(IObserver<Character> o in observers)
            {
                tt += o.GetType().ToString();
            }
            tt += "]";
            return string.Format("CID: {0}, HP: {1}, MP: {2}, Down?: {3}, Buffs: {4}, Observers: {5}", Cb.cid, Hp, Mp, BreakDown, t, tt);
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

        public void MoveTo(int x, int y)
        {
            Vector2Int dir = new Vector2Int(x, y);

            Vector2Int realDir = TransFromY(dir);
            Vector3Int nowPos = TilePos;
            Vector3 des = map.CellToWorld(new Vector3Int(nowPos.x + realDir.x, nowPos.y + realDir.y, nowPos.z));

            if (map.HasTile(map.WorldToCell(des)))
            {
                TilePos = map.WorldToCell(des);
                des.y += 0.1f;

                TCtrl.updateCharNum(map.WorldToCell(des), 1, gameObject);
                TCtrl.updateCharNum(nowPos, -1, gameObject);

                int charsOnDes = TCtrl.getCharList(map.WorldToCell(des)).Count;
                int charsOnCur = TCtrl.getCharList(nowPos).Count;

                worldPos = des;
                destination = des;
                //nowMove = true;
                //Debug.Log("nowpos = " + nowPos + ", despos = " + map.WorldToCell(des));
                //Debug.Log(gameObject.name + ": desNum->" + charsOnDes + ", curNum->" + charsOnCur);

                /*if (map.CellToWorld(nowPos).x < des.x)
                    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                else if(map.CellToWorld(nowPos).x > des.x)
                    gameObject.GetComponent<SpriteRenderer>().flipX = true;*/

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

                    //Debug.Log("noone on tile");
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

        public void Teleport(Vector3Int vec)
        {
            if (map.HasTile(vec))
            {
                Vector3Int nowPos = TilePos;
                TilePos = vec;
                Vector3 newPos = map.CellToWorld(vec);
                newPos.y += 0.1f;
                transform.position = newPos;

                TCtrl.updateCharNum(vec, 1, gameObject);
                TCtrl.updateCharNum(nowPos, -1, gameObject);

                int charsOnDes = TCtrl.getCharList(vec).Count;
                int charsOnCur = TCtrl.getCharList(nowPos).Count;

                worldPos = newPos;



                //if (map.CellToWorld(nowPos).x < des.x)
                //    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                //else if (map.CellToWorld(nowPos).x > des.x)
                //    gameObject.GetComponent<SpriteRenderer>().flipX = true;

                if (charsOnDes > 1)
                {
                    map.SetTileFlags(vec, TileFlags.None);
                    map.SetColor(vec, new Color(1, 1, 1, 0));

                    Sprite sprite = map.GetTile<CustomTile>(vec).sprite;
                    TCtrl.activateAltTile(worldPos, charsOnDes, sprite);

                    List<GameObject> characters = TCtrl.getCharList(vec);

                    int count = 0;

                    foreach (GameObject chara in characters)
                    {
                        Vector2 offset = TCtrl.nList[charsOnDes - 1].coordList[count];
                        chara.GetComponent<Character>().destination = (Vector2)chara.GetComponent<Character>().worldPos + offset;
                        chara.GetComponent<Character>().nowMove = true;
                        //chara.GetComponent<BoxCollider2D>().offset = -offset;
                        count++;
                    }
                }
                else
                {
                    //nowMove = true;
                    gameObject.GetComponent<BoxCollider2D>().offset = Vector2.zero;

                    Debug.Log("noone on tile");
                }

                if (charsOnCur > 1)
                {
                    Sprite sprite = map.GetTile<CustomTile>(nowPos).sprite;
                    Vector3 v = map.CellToWorld(nowPos);
                    v.y += 0.1f;
                    TCtrl.activateAltTile(vec, charsOnCur, sprite);

                    List<GameObject> characters = TCtrl.getCharList(nowPos);

                    int count = 0;
                    //destination = worldPos;
                    foreach (GameObject chara in characters)
                    {
                        Vector3 charpos = chara.transform.position;
                        Vector2 offset = TCtrl.nList[charsOnCur - 1].coordList[count];
                        chara.GetComponent<Character>().destination = (Vector2)chara.GetComponent<Character>().worldPos + offset;
                        chara.GetComponent<Character>().nowMove = true;
                        //chara.GetComponent<BoxCollider2D>().offset = -offset;

                        count++;
                    }
                }

                else if (charsOnCur == 1)
                {
                    map.SetTileFlags(nowPos, TileFlags.None);
                    map.SetColor(nowPos, new Color(1, 1, 1, 1));

                    TCtrl.deactivateAltTile(map.CellToWorld(nowPos));

                    List<GameObject> characters = TCtrl.getCharList(nowPos);
                    characters[0].GetComponent<Character>().destination = map.CellToWorld(nowPos);
                    characters[0].GetComponent<Character>().nowMove = true;
                    characters[0].GetComponent<BoxCollider2D>().offset = Vector2.zero;
                    GetComponent<BoxCollider2D>().offset = Vector2.zero;

                }

                Debug.LogFormat("{0} / {1} teleported to {2}", PhotonNetwork.IsMasterClient ? 'M' : 'C', Cb.cid, vec);
            }
            else
            {
                Debug.LogFormat("{0} / {1} can not go to {2}", PhotonNetwork.IsMasterClient ? 'M' : 'C', Cb.cid, vec);
            }
        }

        public void SetMoveIdx(int value)
        {
            if (value != 0)
            {
                moveIdx += value;
            }
            else
            {
                moveIdx = 0;
            }
        }

        public void SpellSkill(SID sid, SkillDicection direction, int x, int y)
        {
            nowMove = false;
            SkillBase SelSkill = SkillManager.GetData(sid);
            if (SelSkill.areaAttack)
            {
                Vector3 v = map.CellToWorld(new Vector3Int(x, y, 0));
                skillSpawner = SelSkill.area;
                skillSpawner.Activate(new Vector2(v.x, v.y));
                skillSpawner.Destroy(SkillManager.GetData(sid).triggerTime);   // triggerTime만큼 스킬 지속후 삭제
            }
            else
            {
                if (direction == SkillDicection.Right)
                {
                    ray.CurvedMultipleRay(map.CellToWorld(TilePos), SelSkill, SelSkill.directions, true, SelSkill.directions.Count);
                }
                else
                {
                    ray.CurvedMultipleRay(map.CellToWorld(TilePos), SelSkill, SelSkill.directions, false, SelSkill.directions.Count);
                }
            }
            Debug.LogFormat("{0} / {1} spells {2}", PhotonNetwork.IsMasterClient ? 'M' : 'C', Cb.cid, sid);
        }

        #endregion
        

        #region MonoBehaviour CallBacks
        private void Awake()
        {
            Init();


            map = GameObject.FindGameObjectWithTag("Map").GetComponent<Tilemap>();
            TCtrl = GameObject.Find("TilemapControl").GetComponent<TilemapControl>();

            TilePos = map.WorldToCell(transform.position);
            SetMoveIdx(0);
        }

        private void Start()
        {
            GameObject o = GameObject.Find("RayTest");

            if (!o)
            {
                Debug.LogError("Can not find GameObject named RayTest");
            }

            ray = o.GetComponent<RayTest>();

            if (!ray)
            {
                Debug.LogError("Can not find component, RayTest at RayTest");
            }
        }


        void Update()
        {
            if (nowMove)
            {
                transform.position = Vector3.Lerp(gameObject.transform.position, destination, movementSpeed);
                if (transform.position == new Vector3(destination.x, destination.y, 0))
                    nowMove = false;
            }
            else
            {
                
            }
        }



        #endregion

        #region IObserver Methods

        public void AddObserver(IObserver<Character> o)
        {
            if (observers.IndexOf(o) < 0)
            {
                observers.Add(o);
            }
            else
            {
                Debug.LogWarning($"The observer already exists in list: {o}");
            }
        }

        public void RemoveObserver(IObserver<Character> o)
        {
            int idx = observers.IndexOf(o);
            if (idx >= 0)
            {
                observers.RemoveAt(idx); // O(n)
            }
            else
            {
                Debug.LogError($"Can not remove the observer; It does not exist in list: {o}");
            }
        }

        public void NotifyObservers()
        {
            foreach (IObserver<Character> o in observers)
            {
                o.OnNotify(this);
            }
        }

        public void RemoveAllObservers()
        {
            observers.Clear();
        }

        #endregion

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
        }
    }
}
