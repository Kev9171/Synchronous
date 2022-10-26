using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class CharaDataForPick
    {
        public CharaDataForPick(CID cid, int x, int y, Team team)
        {
            this.cid = cid;
            loc = new Vector3Int(x, y, 0);
            this.team = team;
        }

        public CID cid;
        public Vector3Int loc;
        public Team team;
    }
}
