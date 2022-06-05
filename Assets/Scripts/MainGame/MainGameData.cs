using UnityEngine;

using System.Collections.Generic;

namespace KWY
{
    public class MainGameData : MonoBehaviour
    {
        [SerializeField]
        LogicData logicData;


        #region Immutable Variables

        [Tooltip("Player Name or Player NickName")]
        private string playerName;

        public float TimeLimit { get; private set; }
        public int playerMPIncrement { get; private set; }
        public int characterMPIncrement { get; private set; }
        
        [Tooltip("0: left is my side; 1: right is my side")]
        private bool mySide; 

        

        // temp
        private string mapName; // 나중에 enum으로 바꿔서


        #endregion

        #region Mutable Variables

        [Tooltip("진행 턴 수 = 진행중인 n 번째 턴(start: 1)")]
        private int turnNum;

        [SerializeField]
        public int mp { get; internal set; } = 0;


        [Tooltip("Pre-set Possible-to-use Playerskills")]
        [SerializeField]
        private List<PSID> _playerSkillList;

        [Tooltip("Characters under user's control")]
        [SerializeField]
        private List<CharacterBase> _characterList;

        private List<Character> _characters = new List<Character>(); // 게임 진행 중 캐릭터 정보를 가지고 있는 리스트


        // test
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

        public List<CharacterBase> CharacterData { get { return _characterList; } }

        public List<Character> Characters { get { return _characters; } }

        public List<PSID> PlayerSkillList { get { return _playerSkillList; } }


        #endregion

        #region Public Methods

        // test
        public List<Buff> CharaBuffList(CID cid)
        {
            for (int i = 0; i < _characterList.Count; i++)
            {
                if (cid == _characterList[i].cid)
                {
                    switch (i)
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

            if (logicData == null)
            {
                // 할 당안되있으면 직접 로드
                logicData = Resources.Load<LogicData>("MainGameLogicData");
                Debug.Log("Logic Data is loaded from resources");
                if (logicData == null)
                {
                    Debug.LogError("Logic Data is null");
                    return;
                }
            }

            this.TimeLimit = logicData.timeLimit;


        }

        private void Start()
        {
            turnNum = 1;

            _characters.Add(TestCharacter()); // test 
        }

        private Character TestCharacter()
        {
            Character c = new Character(CharaManager.GetData(CID.Flappy));
            c.DamageHP(30);
            c.AddBuff(BuffManager.GetData(BID.Burn), 3);
            c.AddBuff(BuffManager.GetData(BID.Paralyzed), 1);

            Debug.Log(c);

            return c;
        }

        #endregion
    }
}
