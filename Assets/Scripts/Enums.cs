using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    // not editable
    public enum ActionType
    {
        Skill, Move, None
    }

    public enum SkillDicection
    {
        Left, Right
    }

    public enum AID : int
    {
        // temp
        DoubleMove = 1,

    }
    public enum CID : int
    {
        // temp
        Flappy = 1,
        Flappy2 = 2,
        Knight = 3
    }

    public enum SID : int
    {
        FireBall = 1,
        LightingVolt = 2,
        KnightNormal = 3,
        KnightSpecial = 4,
    }

    public enum BID : int
    {
        Burn = 1,
        Paralyzed = 2,
    }

    public enum PSID : int
    {
        Flash = 1,
        Meteor = 2,
    }

    // À°°¢Çü ¹æÇâ
    public enum Direction : int
    {
        TopLeft = 0,
        Left = 1,
        BottomLeft = 2,
        BottomRight = 3,
        Right = 4,
        TopRight = 5,
        Base = 6
    }
}
