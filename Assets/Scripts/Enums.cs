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
        Knight = 3,
        Spearman = 4,
        Healer = 5
    }

    public enum SID : int
    {
        FireBall = 1,
        LightingVolt = 2,
        KnightNormal = 3,
        KnightSpecial = 4,
        Heal = 5,
        SpearAttack = 6
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

    public enum STATE : int
    {
        IDLE = 0,
        StandBy = 1,
        TurnReady = 2,
        Simul = 3,
        GameOver = 4
    }

    // ������ ����
    public enum Direction : int
    {
        None = -1,
        TopLeft = 0,
        Left = 1,
        BottomLeft = 2,
        BottomRight = 3,
        Right = 4,
        TopRight = 5,
        Base = 6
    }

    public enum ErrorCode : int
    {
        PHOTON_DISCONNECT = 0,
        WEB_REQUEST_CONNECTION_ERROR = 101,
        WEB_REQUEST_PROTOCOL_ERROR = 102,
        WEB_REQUEST_ERROR = 103,
        CANNOT_FIND_FILE = 201,
    }

    public enum Team : short
    {
        A = 0,
        B = 1,
    }
    public enum TICK_RESULT
    {
        DRAW = 1,
        MASTER_WIN = 2,
        CLIENT_WIN = 3
    };

    public enum WINLOSE
    {
        WIN = 1,
        LOSE = 2,
        DRAW = 3
    }
}
