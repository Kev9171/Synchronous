using UnityEngine;

using System.Collections.Generic;

using Photon.Pun;

namespace KWY
{
    public class MainGameData : MonoBehaviour
    {
        // for prototype
        [SerializeField]
        GameObject[] tCharas = new GameObject[6];

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
        public int PlayerMp { get; internal set; } = 0;


        [Tooltip("Pre-set Possible-to-use Playerskills")]
        [SerializeField]
        private List<PSID> _playerSkillList;


        private List<Character> _characters = new List<Character>(); // 게임 진행 중 캐릭터 정보를 가지고 있는 리스트
        private Dictionary<CID, GameObject> _charaObjects = new Dictionary<CID, GameObject>();

        private Dictionary<CID, CharacterActionData> _charaActionData = new Dictionary<CID, CharacterActionData>();

        private Dictionary<int, Character> _wholeCharacters = new Dictionary<int, Character>();



        #endregion

        #region Public Fields

        public List<Character> Characters { get { return _characters; } }
        public Dictionary<CID, GameObject> CharacterObjects { get { return _charaObjects; } }
        public Dictionary<CID, CharacterActionData> CharaActionData { get { return _charaActionData; } }

        public Dictionary<int, Character> WholeCharacters { get { return _wholeCharacters; } }


        public List<PSID> PlayerSkillList { get { return _playerSkillList; } }


        #endregion

        #region Public Methods

        public Character GetCharacter(CID cid)
        {
            foreach(Character c in Characters)
            {
                if (c.Cb.cid == cid)
                {
                    return c;
                }
            }
            return null;
        }

        public int GetCharacterNth(CID cid)
        {
            for (int i=0; i<Characters.Count; i++)
            {
                if (Characters[i].Cb.cid == cid)
                {
                    return i;
                }
            }

            return -1;
        }

        public CharacterActionData GetActionData(CID cid)
        {
            if (_charaActionData.TryGetValue(cid, out var value))
            {
                return value;
            }

            Debug.Log("There is no data: " + cid);
            return null;
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            if (logicData == null)
            {
                // 할당안되있으면 직접 로드
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

            //_characters.Add(TestCharacter()); // test 

            // for prototype
            if (PhotonNetwork.IsMasterClient)
            {
                // 0 1 2 추가
                _characters.Add(tCharas[0].GetComponent<Character>());
                _characters.Add(tCharas[1].GetComponent<Character>());
                _characters.Add(tCharas[2].GetComponent<Character>());

                _charaObjects.Add(_characters[0].Cb.cid, tCharas[0]);
                _charaObjects.Add(_characters[1].Cb.cid, tCharas[1]);
                _charaObjects.Add(_characters[2].Cb.cid, tCharas[2]);
            }
            else
            {
                _characters.Add(tCharas[3].GetComponent<Character>());
                _characters.Add(tCharas[4].GetComponent<Character>());
                _characters.Add(tCharas[5].GetComponent<Character>());

                _charaObjects.Add(_characters[0].Cb.cid, tCharas[3]);
                _charaObjects.Add(_characters[1].Cb.cid, tCharas[4]);
                _charaObjects.Add(_characters[2].Cb.cid, tCharas[5]);
            }

            foreach (CID cid in _charaObjects.Keys)
            {
                _charaActionData.Add(cid, new CharacterActionData());
            }

            // test
            _wholeCharacters.Add((int)(_characters[0].Cb.cid), tCharas[0].GetComponent<Character>());
            _wholeCharacters.Add((int)(_characters[1].Cb.cid), tCharas[0].GetComponent<Character>());
            _wholeCharacters.Add((int)(_characters[2].Cb.cid), tCharas[0].GetComponent<Character>());

            _wholeCharacters.Add((int)(_characters[0].Cb.cid) + 100, tCharas[0].GetComponent<Character>());
            _wholeCharacters.Add((int)(_characters[1].Cb.cid) + 100, tCharas[0].GetComponent<Character>());
            _wholeCharacters.Add((int)(_characters[2].Cb.cid) + 100, tCharas[0].GetComponent<Character>());

        }


        private Character TestCharacter()
        {
            Character c = new Character(CharaManager.GetData(CID.Flappy));
            c.DamageHP(300);
            c.AddBuff(BuffManager.GetData(BID.Burn), 3);
            c.AddBuff(BuffManager.GetData(BID.Paralyzed), 1);

            Debug.Log(c);

            return c;
        }

        #endregion
    }
}
