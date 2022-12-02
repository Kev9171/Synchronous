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

        public float HpBarRelativePosY;

        private readonly List<IObserver<Character>> observers = new List<IObserver<Character>>();
        private readonly Dictionary<string, IObserver<Character>> obDict = new Dictionary<string, IObserver<Character>>();

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

        private RayTest ray;

        public bool BreakDownNotice = false;

        public Vector3Int TempTilePos { get; private set; }

        public Vector3 worldPos { get; private set; }

        private readonly float movementSpeed = 1f;
        public float currentTime = 0;
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

        private void BreakDownStatus()
        {
            ClearBuff();
            GetComponent<Animator>().SetBool("IsDead", true);
            GetComponent<SpriteRenderer>().color = Color.red;

            if (gameObject.TryGetComponent(out BoxCollider2D c1))
            {
                c1.enabled = false;
            }

            RemoveAllObservers();
        }

        public void DamageHP(int damage)
        {
            if (BreakDown)
            {
                return;
            }

            if (Hp - damage > 0)
            {
                Hp -= damage;
                Debug.LogFormat($"[id={Pc.Id}]{Cb.name} is damaged {damage}; Now hp: {Hp}");
            }
            else if (Hp - damage <= 0)
            {
                Hp = 0;
                BreakDown = true;
                Debug.LogFormat($"[id={Pc.Id}]{Cb.name} is damaged {damage}; Now hp: {Hp}, BREAK DOWN!!");
            }

            NotifyObservers();

            if (BreakDown)
            {
                BreakDownStatus();
            }
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
        [PunRPC]
        private void SetTilePosRPC(int x, int y)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                TilePos = new Vector3Int(x, y, 0);
            }
        }

        public void ResetTempPosAndMp()
        {
            //TempTilePos = map.WorldToCell(transform.position);
            TempTilePos = TilePos;
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

        [PunRPC]
        private void SetAnimatorTriggerAttack()
        {
            GetComponent<Animator>().SetTrigger("IsAttacking"); // Activates animation.
        }

        [PunRPC]
        private void SetAnimatorTriggerJump()
        {
            GetComponent<Animator>().SetTrigger("IsJumping"); // Activates animation.
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
            if (BreakDown) return;

            Vector2Int dir = new Vector2Int(x, y);

            Vector2Int realDir = TransFromY(dir);
            Vector3Int nowPos = TilePos;
            worldPos = map.CellToWorld(nowPos);
            Vector3 des = map.CellToWorld(new Vector3Int(nowPos.x + realDir.x, nowPos.y + realDir.y, nowPos.z));

            if (map.HasTile(map.WorldToCell(des)))
            {
                TilePos = map.WorldToCell(des);
                des.y += 0.1f;

                TCtrl.updateCharNum(TilePos, 1, gameObject);
                TCtrl.updateCharNum(nowPos, -1, gameObject);

                int charsOnDes = TCtrl.getCharList(TilePos).Count;
                int charsOnCur = TCtrl.getCharList(nowPos).Count;

                destination = des;
                //worldPos = des;

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
                    map.SetTileFlags(TilePos, TileFlags.None);
                    map.SetColor(TilePos, new Color(1, 1, 1, 0));

                    Sprite sprite = map.GetTile<CustomTile>(TilePos).sprite;
                    TCtrl.activateAltTile(des, charsOnDes, sprite);

                    List<GameObject> characters = TCtrl.getCharList(TilePos);

                    int count = 0;

                    //nowMove = true;
                    foreach (GameObject chara in characters)
                    {
                        Vector2 offset = TCtrl.nList[charsOnDes - 1].coordList[count];
                        chara.GetComponent<Character>().destination += offset; //(Vector2)chara.GetComponent<Character>().worldPos + offset;
                        chara.GetComponent<Character>().nowMove = true;
                        //chara.GetComponent<BoxCollider2D>().offset = -offset;

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
                        chara.GetComponent<Character>().destination += offset; //(Vector2)chara.GetComponent<Character>().worldPos + offset;
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

        [PunRPC]
        public void Teleport(int x, int y, int id, bool yDiff)
        {
            if (BreakDown) return;

            Vector3Int vec = new Vector3Int(x, y, 0);

            if (map.HasTile(vec))
            {
                if (yDiff)
                {
                    if (GameManager.Instance.Simulation.actionData.Data.TryGetValue(id, out var value))
                    {
                        int y2 = y;
                        for (int i = GameManager.Instance.Simulation.data.PCharacters[id].Chara.moveIdx; i < value.Length; i++)
                        {
                            object[] d = (object[])value[i];
                            if ((ActionType)d[1] == ActionType.Move)
                            {
                                Vector2Int vec2 = new Vector2Int((int)d[2], (int)d[3]);
                                List<Vector2Int> v = y2 % 2 != 0 ? MoveManager.MoveData.areaEvenY : MoveManager.MoveData.areaOddY;
                                //List<Vector2Int> cur = y % 2 != 0 ? action.areaEvenY : action.areaOddY;
                                int idx = v.IndexOf(vec2);
                                Vector2Int newVec = v[5 - idx] * (-1);
                                Debug.Log("vec = " + vec2 + ", newvec = " + newVec + "index = " + idx);
                                Debug.Log("ActionType = " + d[1] + ", vec = " + d[2] + ", " + d[3]);
                                d[2] = newVec.x;
                                d[3] = newVec.y;
                                value[i] = d;
                                y2 += (int)d[3];
                                Debug.Log("index: " + i + ", value[i] " + value[i] + ", moveIdx: " + GameManager.Instance.Simulation.data.PCharacters[id].Chara.moveIdx);
                            }
                        }
                        GameManager.Instance.Simulation.actionData.Data[id] = value;
                    }
                    else
                        Debug.Log("no char matching cid");
                }

                Vector3Int nowPos = TilePos;
                TilePos = vec;
                photonView.RPC("SetTilePosRPC", RpcTarget.Others, vec.x, vec.y);
                Vector3 newPos = map.CellToWorld(vec);
                newPos.y += 0.1f;
                transform.position = newPos;

                TCtrl.updateCharNum(vec, 1, gameObject);
                TCtrl.updateCharNum(nowPos, -1, gameObject);

                int charsOnDes = TCtrl.getCharList(vec).Count;
                int charsOnCur = TCtrl.getCharList(nowPos).Count;

                worldPos = map.CellToWorld(nowPos); //newPos

                //GetComponentInParent<SpawnEffect>().PlayEffect();
                photonView.RPC("PlayEffect", RpcTarget.All);
                //if (map.CellToWorld(nowPos).x < des.x)
                //    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                //else if (map.CellToWorld(nowPos).x > des.x)
                //    gameObject.GetComponent<SpriteRenderer>().flipX = true;

                if (charsOnDes > 1)
                {
                    map.SetTileFlags(vec, TileFlags.None);
                    map.SetColor(vec, new Color(1, 1, 1, 0));

                    Sprite sprite = map.GetTile<CustomTile>(vec).sprite;
                    TCtrl.activateAltTile(newPos, charsOnDes, sprite);

                    List<GameObject> characters = TCtrl.getCharList(vec);

                    int count = 0;

                    foreach (GameObject chara in characters)
                    {
                        Vector2 offset = TCtrl.nList[charsOnDes - 1].coordList[count];
                        chara.GetComponent<Character>().destination += offset; //(Vector2)chara.GetComponent<Character>().worldPos + offset;
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
                    foreach (GameObject chara in characters)
                    {
                        Vector3 charpos = chara.transform.position;
                        Vector2 offset = TCtrl.nList[charsOnCur - 1].coordList[count];
                        chara.GetComponent<Character>().destination += offset; //(Vector2)chara.GetComponent<Character>().worldPos + offset;
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

        public void SpellSkill(SID sid, Direction direction, int x, int y)
        {
            if (BreakDown) return;

            photonView.RPC("SetAnimatorTriggerAttack", RpcTarget.All);
            nowMove = false;
            SkillBase SelSkill = SkillManager.GetData(sid);

            DataController.Instance.ModifyCharacterMp(Pc.Id, -SelSkill.cost);

            if (SelSkill.areaAttack)
            {
                Vector3 v = map.CellToWorld(new Vector3Int(x, y, 0));
                GameObject o = PhotonNetwork.Instantiate(
                    SpawnableSkillResources.GetPath(SelSkill.sid),
                    new Vector3(v.x, v.y + 0.1f, 0),
                    Quaternion.identity);

                if (!NullCheck.HasItComponent<SkillSpawner>(o, "SkillSpawner")) {
                    // error
                    return;
                }

                o.GetComponent<SkillSpawner>().Init(Pc.Team, Atk);


                //skillSpawner = SelSkill.area;
                //skillSpawner.Activate(new Vector2(x, y), Pc.Team, Atk);
                //skillSpawner.Destroy(SkillManager.GetData(sid).triggerTime);   // triggerTime만큼 스킬 지속후 삭제
            }
            else
            {
                if(Pc.Team == 0)
                {
                    ray.CurvedMultipleRay(map.CellToWorld(TilePos), SelSkill, SelSkill.directions, (int)direction, false, SelSkill.directions.Count);
                }
                else
                {
                    ray.CurvedMultipleRay(map.CellToWorld(TilePos), SelSkill, SelSkill.directions, (int)direction, true, SelSkill.directions.Count);
                }
            }
            Debug.LogFormat("{0} / {1} spells {2}", PhotonNetwork.IsMasterClient ? 'M' : 'C', Cb.cid, sid);
        }

        public void SkillSwitcher(SkillBase SelSkill)
        {
            switch (SelSkill.sid)
            {
                case SID.FireBall:
                    break;
                case SID.LightingVolt:
                    break;
                case SID.KnightNormal:
                    //GameObject Arrow = PhotonNetwork.Instantiate("Arrow", transform.position, Quaternion.identity, 0);
                    //Arrow arrow = GetComponent<Arrow>();
                    //arrow.targetPosition.x = transform.position.x - 6f;
                    Debug.Log("Spawn an arrow.");
                    break;
                case SID.KnightSpecial:
                    photonView.RPC("SetAnimatorTriggerJump", RpcTarget.All);
                    break;
                default:
                    break;
            }
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
            if (!PhotonNetwork.InRoom)
            {
                return;
            }

            if (BreakDown) return;

            //if (!PhotonNetwork.IsMasterClient)
            //{
            //    return;
            //}

            if (nowMove)
            {
                GetComponent<Animator>().SetBool("IsMoving", true); // Enables animation of movement.
                currentTime += Time.deltaTime;
                if (currentTime >= movementSpeed)
                    currentTime = movementSpeed;
                transform.position = Vector3.Lerp(worldPos/*transform.position*/, destination, currentTime/movementSpeed);
                if (transform.position == new Vector3(destination.x, destination.y, 0))
                {
                    GetComponent<Animator>().SetBool("IsMoving", false); // Stops movement animation.
                    nowMove = false;
                    currentTime = 0;
                }
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

                obDict.Add(o.GetType().Name, o);
            }
            else
            {
                Debug.LogWarning($"The observer already exists in list: {o}");
            }
        }

        public void RemoveObserver(string observerName)
        {
            RemoveObserver(obDict[observerName]);
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
