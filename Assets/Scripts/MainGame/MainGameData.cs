using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections.Generic;

using Photon.Pun;

namespace KWY
{
    public class MainGameData : MonoBehaviour, ISubject
    {
        public List<IObserver> Observers
        {
            get;
            private set;
        } = new List<IObserver>();

        public float TimeLimit { get; private set; }

        // temp
        private string mapName; // 나중에 enum으로 바꿔서

        private int turnNum = 0;

        public int TurnNum
        {
            get
            {
                return turnNum;
            }
            set
            {
                turnNum = value;
                NotifyObservers();
            }
        }

        public int PlayerMp { get; internal set; } = 0;

        [Tooltip("Pre-set Possible-to-use Playerskills")]
        [SerializeField]
        private List<PSID> _playerSkillList;

        [SerializeField]
        Player player;

        private Tilemap _tileMap;

        private List<Character> _characters = new List<Character>(); // 게임 진행 중 캐릭터 정보를 가지고 있는 리스트
        private Dictionary<CID, GameObject> _charaObjects = new Dictionary<CID, GameObject>();

        

        private Dictionary<int, Character> _wholeCharacters = new Dictionary<int, Character>();



        private Dictionary<int, CharacterActionData> _charaActionData = new Dictionary<int, CharacterActionData>();
        // 필드에 있는 캐릭터 정보를 가지고 있는 Dictionary
        private Dictionary<int, PlayableCharacter> _charactersDict = new Dictionary<int, PlayableCharacter>();
        private List<PlayableCharacter> _charasTeamA = new List<PlayableCharacter>();
        private List<PlayableCharacter> _charasTeamB = new List<PlayableCharacter>();


        #region Public Fields

        public List<Character> Characters { get { return _characters; } }
        public Dictionary<CID, GameObject> CharacterObjects { get { return _charaObjects; } }
        
        public Dictionary<int, Character> WholeCharacters { get { return _wholeCharacters; } }
        public List<PSID> PlayerSkillList { get { return _playerSkillList; } }


        public Dictionary<int, CharacterActionData> CharaActionData { get { return _charaActionData; } }
        public Dictionary<int, PlayableCharacter> CharactersDict { get { return _charactersDict; } }
        public List<PlayableCharacter> CharasTeamA { get { return _charasTeamA; } }
        public List<PlayableCharacter> CharasTeamB { get { return _charasTeamB; } }
        public List<PlayableCharacter> MyTeamCharacter
        {
            get
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    return _charasTeamA;
                }
                else
                {
                    return _charasTeamB;
                }
            }
        }

        public Player MyPlayer
        {
            get
            {
                return player;
            }
        }

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

        public PlayableCharacter GetPlayableCharacter(Character chara)
        {
            foreach(PlayableCharacter c in MyTeamCharacter)
            {
                if (c.Chara.Cb.cid.Equals(chara.Cb.cid))
                {
                    return c;
                }
            }
            return null;
        }

        public CharacterActionData GetActionData(int id)
        {
            if (_charaActionData.TryGetValue(id, out var value))
            {
                return value;
            }

            Debug.Log("There is no data: " + id);
            return null;
        }

        public void LoadData()
        {

            this.TimeLimit = LogicData.Instance.TimeLimit;
            this.PlayerMp = LogicData.Instance.PlayerInitialMp;

            //_characters.Add(TestCharacter()); // test 

            /*// for prototype
            if (PhotonNetwork.IsMasterClient)
            {
                // 캐릭터 태그 추가
                for (int i = 0; i < tCharas.Length; i++)
                {
                    if (i < 3)
                    {
                        tCharas[i].tag = "Friendly";
                        tCharas[i].layer = 7;
                    }
                    else
                    {
                        tCharas[i].tag = "Enemy";
                        tCharas[i].layer = 6;
                    }
                }

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
                for (int i = 0; i < tCharas.Length; i++)
                {
                    if (i < 3)
                    {
                        tCharas[i].tag = "Enemy";
                        tCharas[i].layer = 6;
                    }
                    else
                    {
                        tCharas[i].tag = "Friendly";
                        tCharas[i].layer = 7;
                    }
                }

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
            _wholeCharacters.Add((int)(_characters[1].Cb.cid), tCharas[1].GetComponent<Character>());
            _wholeCharacters.Add((int)(_characters[2].Cb.cid), tCharas[2].GetComponent<Character>());

            _wholeCharacters.Add(((int)(_characters[0].Cb.cid)) + 100, tCharas[3].GetComponent<Character>());
            _wholeCharacters.Add(((int)(_characters[1].Cb.cid)) + 100, tCharas[4].GetComponent<Character>());
            _wholeCharacters.Add(((int)(_characters[2].Cb.cid)) + 100, tCharas[5].GetComponent<Character>());*/


            // DontDestroyOnLoad 에 있는 캐릭터들 좌표와 타입 가져오기
            // 일단 아래 내용으로 가져왔다고 치고
            List<CharaData> tList = new List<CharaData>
            {
                new CharaData(CID.Flappy, -3, 0, Team.A),
                new CharaData(CID.Flappy2, -3, 1, Team.A),
                new CharaData(CID.Knight, -3, 2, Team.A),

                new CharaData(CID.Flappy, 5, 0, Team.B),
                new CharaData(CID.Flappy2, 5, 1, Team.B),
                new CharaData(CID.Knight, 5, 2, Team.B),
            };

            List<GameObject> tCharaObjects = new List<GameObject>();
            foreach(CharaData d in tList)
            {
                GameObject g = CharacterResources.LoadCharacter(d.cid);
                GameObject chara = null;

                if (g)
                {
                    chara = Instantiate(g, _tileMap.CellToWorld(d.loc), Quaternion.identity);

                    // B 팀(2nd client)일 경우 x 축 반전으로 
                    if (d.team == Team.B)
                    {
                        SpriteRenderer t = chara.GetComponent<SpriteRenderer>();
                        if (t)
                        {
                            t.flipX = true;
                        }
                        else
                        {
                            Debug.LogError($"Can not find component SpriteRenderer in character; CID: {d.cid}");
                        }
                    }
                    else
                    {
                    }
                }
                else
                {
                    Debug.LogError($"Could not instantiate character object; CID: {d.cid}");
                }

                // Instantiate 된 캐릭터를 dictionary에 추가
                int id = IdHandler.GetNewId();
                PlayableCharacter pc = new PlayableCharacter(chara, id, d.team);
                _charactersDict.Add(id, pc);
                chara.GetComponent<Character>().SetData(pc);

                // 팀에 맞게 리스트에 추가
                if (d.team == Team.A)
                {
                    _charasTeamA.Add(pc);
                }
                else if (d.team == Team.B)
                {
                    _charasTeamB.Add(pc);
                }

                _charaActionData.Add(id, new CharacterActionData());
            }


            InitBaseObservers();
            // 확인용 코드
            /*foreach (PlayableCharacter c in _charactersDict.Values)
            {
                Debug.Log(c);
            }*/

            /*foreach (PlayableCharacter c in _playableDict.Values)
            {
                Debug.Log(c.Chara.GetHashCode());
            }*/
        }


        /// <summary>
        /// Must be called after LoadData()
        /// </summary>
        private void InitBaseObservers()
        {
            // add observer

            // character
            foreach (PlayableCharacter p in _charactersDict.Values)
            {
                p.Chara.AddObserver(new CharacterObserver());
            }

            // player
            player.AddObserver(new PlayerObserver());

            // main data
            AddObserver(new GameProgressObserver());
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            GameObject t = GameObject.FindGameObjectWithTag("Map");
            if (t)
            {
                _tileMap = t.GetComponent<Tilemap>();
            }
            else
            {
                Debug.LogError("Can not find object with tag: 'Map'");
            }
        }

        private void Start()
        {
        }


        #endregion

        #region ISubject Methods
        public void AddObserver(IObserver o)
        {
            if (Observers.IndexOf(o) < 0)
            {
                Observers.Add(o);
            }
            else
            {
                Debug.LogWarning($"The observer already exists in list: {o}");
            }
        }

        public void RemoveObserver(IObserver o)
        {
            int idx = Observers.IndexOf(o);
            if (idx >= 0)
            {
                Observers.RemoveAt(idx); // O(n)
            }
            else
            {
                Debug.LogError($"Can not remove the observer; It does not exist in list: {o}");
            }
        }

        public void NotifyObservers()
        {
            foreach (IObserver o in Observers)
            {
                o.OnNotify();
            }
        }

        public void RemoveAllObservers()
        {
            Observers.Clear();
        }
        #endregion
    }

    // temp
    class CharaData
    {
        public CharaData(CID cid, int x, int y, Team team)
        {
            this.cid = cid;
            loc = new Vector3Int(x, y, 0);
            this.team = team;
        }

        public CID cid;
        public Vector3Int loc;
        public Team team;
    }

    class IdHandler
    {
        static int id = 0;
        static readonly object _lock = new object();

        public static int GetNewId()
        {
            int v = 0;

            lock (_lock)
            {
                v = id;
                id++;
            }

            return v;
        }
    }
}
