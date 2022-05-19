using Photon.Pun;

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

        public void Damage(float damage);
        public void MoveTo(int row, int col);
        public void CastSkill();
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
