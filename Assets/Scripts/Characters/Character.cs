using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

namespace KWY
{
    public class Character {

        public CharacterBase Cb { get; private set; }
        public List<Buff> Buffs { get; private set; }
        public float Hp { get; private set; }
        public float Mp { get; private set; }
        public bool BreakDown { get; private set; }

        public const float MaxMp = 10;

        public Character(CharacterBase cb)
        {
            Cb = cb;
            Buffs = new List<Buff>();
            Hp = cb.hp;
            Mp = 0;
            BreakDown = false;
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

    }
    public interface ICharacter 
    {
        CClass CLASS { get; }
        float HP { get; }
        float ATK { get; }

        float YCorrectionValue { get; }

        Dictionary<int, string> Moves { get; set; }

        int moveCnt { get;set; }

        public void Damage(float damage);
        public void MoveTo(int row, int col);
        public void CastSkill();

        //public void OnClick();
    }

    // The claases of characters that are unique
    public enum CClass
    {
        // temp
        Class_A,
        Class_B,
        Class_C
    }
}
