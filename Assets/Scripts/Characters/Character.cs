using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

namespace KWY
{
    public class Character {

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
