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


        [Tooltip("Pre-set Possible-to-use Playerskills")]
        [SerializeField]
        private List<PSID> _playerSkillList;

        [Tooltip("Characters under user's control")]
        [SerializeField]
        private List<CharacterBase> _characters;




        [SerializeField]
        private List<Buff> chara1BuffList = new List<Buff>();
        [SerializeField]
        private List<Buff> chara2BuffList = new List<Buff>();
        [SerializeField]
        private List<Buff> chara3BuffList = new List<Buff>();

        //for test
        [SerializeField]
        private List<BID> chara1BuffListTest = new List<BID>();




        #endregion

        #region Public Fields

        public List<CharacterBase> CharacterData { get { return _characters; } }

        public List<PSID> PlayerSkillList { get { return _playerSkillList; } }

        public List<Buff> CharaBuffList(CID cid)
        {
            for(int i=0; i<_characters.Count; i++)
            {
                if (cid == _characters[i].cid)
                {
                    switch(i)
                    {
                        case 0: return chara1BuffList;
                        case 1: return chara2BuffList;
                        case 2: return chara3BuffList;
                    }
                }
            }
            return null;
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            // for test
            foreach(BID bid in chara1BuffListTest)
            {
                chara1BuffList.Add(new Buff(BuffManager.GetData(bid), 1));
            }
        }

        #endregion
    }
}
