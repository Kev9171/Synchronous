#define TEST

using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections.Generic;

using Photon.Pun;
using System;

using DebugUtil;

namespace KWY
{
    public class MainGameData : MonoBehaviour, ISubject, ISubject<EGameProgress>
    {
        public static MainGameData Instance;

        private PhotonView photonView;

        public List<IObserver> Observers
        {
            get;
            private set;
        } = new List<IObserver>();

        public List<IObserver<EGameProgress>> ProgressObservers
        {
            get;
            private set;
        } = new List<IObserver<EGameProgress>>();


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
                NotifyObservers(EGameProgress.TURN_NUM);
            }
        }

        [Tooltip("Pre-set Possible-to-use Playerskills")]
        [SerializeField]
        private List<PSID> _playerSkillList;

        [SerializeField]
        Player player;

        private Tilemap _tileMap;

        private readonly Dictionary<int, CharacterActionData> _charaActionData = new Dictionary<int, CharacterActionData>();
        // 필드에 있는 캐릭터 정보를 가지고 있는 Dictionary
        private readonly Dictionary<int, PlayableCharacter> _pCharacters = new Dictionary<int, PlayableCharacter>();
        private readonly List<PlayableCharacter> _charasTeamA = new List<PlayableCharacter>();
        private readonly List<PlayableCharacter> _charasTeamB = new List<PlayableCharacter>();
        private readonly Dictionary<int, bool> _isMyCharacter = new Dictionary<int, bool>();

        private int _notBreakDownTeamA = 0;
        private int _notBreakDownTeamB = 0;


        #region Public Fields        
        public List<PSID> PlayerSkillList { get { return _playerSkillList; } }

        public Dictionary<int, CharacterActionData> CharaActionData { get { return _charaActionData; } }
        public Dictionary<int, PlayableCharacter> PCharacters { get { return _pCharacters; } }
        public List<PlayableCharacter> CharasTeamA { get { return _charasTeamA; } }
        public List<PlayableCharacter> CharasTeamB { get { return _charasTeamB; } }
        public Dictionary<int, bool> IsMyCharacter { 
            get 
            {
                return _isMyCharacter; 
            } 
        }
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

        public List<PlayableCharacter> OtherTeamCharacter
        {
            get
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    return _charasTeamB;
                }
                else
                {
                    return _charasTeamA;
                }
            }
        }

        // TODO
        // 데이터 변경이 없는 상태일 때도 모든 observer에 notify를 하고 있음
        // 특정 옵저버만 notify를 할 수있고
        // 옵저버를 관리해줄 수 있는 클래스 만들기

        public int NotBreakDownTeamA 
        { 
            set { _notBreakDownTeamA = value; NotifyObservers(); }
            get { return _notBreakDownTeamA; } 
        }
        public int NotBreakDownTeamB 
        { 
            set { _notBreakDownTeamB = value; NotifyObservers(); }
            get { return _notBreakDownTeamB; } 
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

            // DontDestroyOnLoad 에 있는 캐릭터들 좌표와 타입 가져오기
            // 일단 아래 내용으로 가져왔다고 치고

            if (PhotonNetwork.IsMasterClient)
            {
                InitCharacters();
            }
            else
            {

            }


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

        private void InitCharacters()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            List<CharaData> tList = new List<CharaData>
            {
                new CharaData(CID.Flappy, -3, 0, Team.A),
                new CharaData(CID.Flappy2, -3, 1, Team.A),
                new CharaData(CID.Knight, -3, 2, Team.A),

                new CharaData(CID.Flappy, 5, 0, Team.B),
                new CharaData(CID.Flappy2, 5, 1, Team.B),
                new CharaData(CID.Knight, 5, 2, Team.B),
            };

            foreach (CharaData d in tList)
            {
                GameObject chara;
                if (chara = PhotonInstantiate(d.cid, d.loc))
                {
                    //chara = PhotonNetwork.Instantiate(g.name, _tileMap.CellToWorld(d.loc), Quaternion.identity);
                    //chara = PhotonNetwork.Instantiate(CharacterResources.Flappy_1, _tileMap.CellToWorld(d.loc), Quaternion.identity);

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
                _pCharacters.Add(id, pc);
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

                if (d.team == Team.A)
                {
                    _charaActionData.Add(id, new CharacterActionData(id, new CharacterActionReadyObserver()));
                    _isMyCharacter.Add(id, true);

                    _notBreakDownTeamA++;

                    chara.tag = "Friendly";
                }
                else
                {
                    _isMyCharacter.Add(id, false);

                    _notBreakDownTeamB++;

                    chara.tag = "Enemy";
                }

                photonView.RPC(
                    "InitCharacterRPC", RpcTarget.Others,
                    chara.GetPhotonView().ViewID, id, ((int)d.team));
            }

            InitBaseObservers();
        }


        [PunRPC]
        private void OnGameReadyRPC()
        {
            Debug.Log("OnGameReadyRPC");
            GameManager.Instance.SetState(STATE.StandBy);
        }

        // serialize custom type으로 바꿔야 할듯...
        // RPC가 비동기 방식으로 작동될 경우 -> list에 add 시 lock을 걸어주어야 할수도?
        // 일단은 하나씩 실행되는 것 처럼 보이므로 lock 사용 안했음

        [PunRPC]
        private void InitCharacterRPC(int viewId, int id, int team) 
        {
            //Debug.Log($"start viewId:{viewId}");
            if (PhotonNetwork.IsMasterClient)
            {
                return;
            }

            Team _team = (Team)team;

            // get object by photonViewId
            PhotonView _photonView = PhotonNetwork.GetPhotonView(viewId);
            if (!_photonView)
            {
                Debug.Log($"Can not find PhotonView id: {id}");
                return;
            }

            GameObject chara = _photonView.gameObject;
            if (!chara)
            {
                Debug.Log($"Can not find gameobject on photonView id: {id}");
                return;
            }

            PlayableCharacter pc = new PlayableCharacter(chara, id, _team);

            _pCharacters.Add(id, pc);
            chara.GetComponent<Character>().SetData(pc);

            // Master Client
            if (_team == Team.A)
            {
                _charasTeamA.Add(pc);
                _isMyCharacter.Add(id, false);

                chara.tag = "Enemy";
            }
            // other client
            else
            {
                _charasTeamB.Add(pc);

                _charaActionData.Add(id, new CharacterActionData(id, new CharacterActionReadyObserver()));
                _isMyCharacter.Add(id, true);

                chara.tag = "Friendly";

                if (NullCheck.HasItComponent(pc.CharaObject, "SpriteRenderer", out SpriteRenderer component))
                {
                    component.flipX = true;
                }
            }

            // for test
#if TEST
            if (_pCharacters.Count == 6)
            {
                Debug.Log("_pCharacters:");
                foreach (PlayableCharacter p in _pCharacters.Values)
                {
                    Debug.Log(p);
                }
            }

            if (_charasTeamB.Count == 3)
            {
                Debug.Log("_myTeamCharacter(B):");
                foreach (PlayableCharacter p in _charasTeamB)
                {
                    Debug.Log(p);
                }
            }
#endif
            //Debug.Log($"end viewId:{viewId}");

            if (_pCharacters.Count == 6)
            {
                InitBaseObservers();
                photonView.RPC("OnGameReadyRPC", RpcTarget.All);
            }

            
        }

        private GameObject PhotonInstantiate(CID cid, Vector3Int loc)
        {
            try
            {
                return cid switch
                {
                    CID.Flappy => PhotonNetwork.Instantiate(CharacterResources.Flappy_1, _tileMap.CellToWorld(loc), Quaternion.identity),
                    CID.Flappy2 => PhotonNetwork.Instantiate(CharacterResources.Flappy2_2, _tileMap.CellToWorld(loc), Quaternion.identity),
                    CID.Knight => PhotonNetwork.Instantiate(CharacterResources.Knight_3, _tileMap.CellToWorld(loc), Quaternion.identity),
                    _ => throw new System.NotImplementedException(),
                };
            } catch (Exception)
            {
                return null;
            }
            
        }


        /// <summary>
        /// Must be called after LoadData()
        /// </summary>
        private void InitBaseObservers()
        {
            // add observer

            // character
            // 일단 자신의 캐릭터만 (현재 UI 업데이트는 자신의 캐릭터만 됨)
            foreach (PlayableCharacter p in MyTeamCharacter)
            {
                p.Chara.AddObserver(new CharacterObserver());
            }

            // player
            player.AddObserver(new PlayerObserver());


            if (PhotonNetwork.IsMasterClient)
            {
                foreach(PlayableCharacter p in PCharacters.Values)
                {
                    p.Chara.AddObserver(new CharacterBreakDownObserver());
                }

                AddObserver(new GameOverObserver());
            }

            AddObserver(new GameProgressObserver());
        }
        #endregion

        
        public ResultData CreateResultData()
        {
            return new ResultData(MyTeamCharacter, MyPlayer);
        }

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            Instance = this;

            IdHandler.ClearId();

            photonView = PhotonView.Get(this);
            if (!photonView)
            {
                Debug.LogError("Can not find photonview on this object (MainGameData)");
                return;
            }

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

        public void NotifyObservers(EGameProgress p)
        {
            foreach(IObserver<EGameProgress> o in ProgressObservers)
            {
                o.OnNotify(p);
            }
        }

        public void AddObserver(IObserver<EGameProgress> o)
        {
            if (ProgressObservers.IndexOf(o) < 0)
            {
                ProgressObservers.Add(o);
            }
            else
            {
                Debug.LogWarning($"The observer already exists in list: {o}");
            }
        }

        public void RemoveObserver(IObserver<EGameProgress> o)
        {
            int idx = ProgressObservers.IndexOf(o);
            if (idx >= 0)
            {
                ProgressObservers.RemoveAt(idx); // O(n)
            }
            else
            {
                Debug.LogError($"Can not remove the observer; It does not exist in list: {o}");
            }
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

        public static void ClearId()
        {
            id = 0;
        }
    }

    class GameProgressSubscriber : ISubject
    {
        public List<IObserver> Observers
        {
            get;
            private set;
        } = new List<IObserver>();

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
    }
}
