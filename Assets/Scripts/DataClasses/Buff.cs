using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class Buff
    {
        public BID bid;
        public BuffBase bb;
        public int turn;

        public Buff(BuffBase bb, int turn)
        {
            this.bb = bb;
            this.turn = turn;
        }

        public override string ToString()
        {
            return string.Format("<{0}, {1}>", bb.name, turn);
        }
    }
}
