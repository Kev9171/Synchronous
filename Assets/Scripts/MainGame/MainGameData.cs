using UnityEngine;

using System.Collections.Generic;

namespace KWY
{
    public class MainGameData : MonoBehaviour
    {
        #region Immutable Variables

        [Tooltip("Player Name or Player NickName")]
        private string playerName;
        [Tooltip("Time limit of TurnReady")]
        private int timeLimit;
        [Tooltip("Pre-set Possible-to-use Playerskills")]
        private PlayerSkill[] playerSkills;
        [Tooltip("0: left is my side; 1: right is my side")]
        private bool mySide; 

        

        // temp
        private string mapName; // 나중에 enum으로 바꿔서


        #endregion

        #region Mutable Variables

        [Tooltip("진행 턴 수 = 진행중인 n 번째 턴(start: 1)")]
        private int turnNum;

        [Tooltip("Player Mana Point")]
        private int mp;

        [Tooltip("Characters under user's control")]
        [SerializeField]
        private List<CharacterBase> _characters;


        #endregion

        #region Public Fields

        public List<CharacterBase> CharacterData { get { return _characters; } }

        #endregion
    }
}
