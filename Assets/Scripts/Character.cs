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
        public Vector3Int SelTilePos { get; private set; }

        public static readonly float MaxMp = 10;

        [SerializeField] private float movementSpeed;
        private Vector2 destination;

        private Tilemap map;

        private bool nowMove = false;

        private skillSpawner skillSpawner;
        [SerializeField] private RayTest ray;
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
            TempTilePos = map.WorldToCell(transform.position);
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

        public void MoveTo(Vector2Int dir)
        {
            map = GameObject.FindGameObjectWithTag("Map").GetComponent<Tilemap>();

            Vector2Int realDir = TransFromY(dir);
            Vector3Int nowPos = map.WorldToCell(this.transform.position);
            Vector3 des = map.CellToWorld(new Vector3Int(nowPos.x + realDir.x, nowPos.y + realDir.y, nowPos.z));

            if (map.HasTile(map.WorldToCell(des)))
            {
                destination = des;
                nowMove = true;
                Debug.LogFormat("{0} / {1} is moving to {2}", PhotonNetwork.IsMasterClient ? 'M' : 'C', Cb.cid, map.WorldToCell(des));
            }
            else
            {
                Debug.LogFormat("{0} / {1} can not go to {2}", PhotonNetwork.IsMasterClient ? 'M' : 'C', Cb.cid, map.WorldToCell(des));         
            }
        }

        public void SpellSkill(SID sid, SkillDicection direction)
        {
            nowMove = false;
            SkillBase SelSkill = SkillManager.GetData(sid);
            if (SelSkill.areaAttack)
            {
                skillSpawner = SelSkill.area;
                Vector3Int v = new Vector3Int(SelTilePos.x, SelTilePos.y, 0);
                skillSpawner.Activate(map.CellToWorld(v));
                skillSpawner.Destroy(SkillManager.GetData(sid).triggerTime);   // triggerTime만큼 스킬 지속후 삭제
            }
            else
            {
                ray.CurvedMultipleRay(map.CellToWorld(TempTilePos), SelSkill, SelSkill.directions, true, SelSkill.directions.Count);
            }
            Debug.LogFormat("{0} / {1} spells {2}", PhotonNetwork.IsMasterClient ? 'M' : 'C', Cb.cid, sid);
        }

        #endregion
        

        #region MonoBehaviour CallBacks
        private void Awake()
        {
            Cb = _characterBase;
            Buffs = new List<Buff>();

            map = GameObject.FindGameObjectWithTag("Map").GetComponent<Tilemap>();

            Hp = Cb.hp; 
            Mp = 0;
            BreakDown = false;

            Debug.Log(this+"'s pos = "+map.WorldToCell(transform.position));
        }

        void Update()
        {
            if (nowMove)
            {
                transform.position = Vector3.Lerp(gameObject.transform.position, destination, 0.7f);
            }
            else
            {

            }
        }
        #endregion


    }
}
