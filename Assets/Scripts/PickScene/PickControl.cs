using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Photon.Pun;
using System;

using Random = UnityEngine.Random;

using KWY;
using UI;

namespace PickScene
{
    public class PickControl : Singleton<PickControl>
    {
        private readonly int NullCID = -1;
        private readonly int SetableNum = 3;

        [SerializeField]
        GameObject Canvas;

        [SerializeField]
        Color HighlightColor;

        [SerializeField] Tilemap map;
        [SerializeField] Tilemap hlMap;
        /*[SerializeField] GameObject ClientCharacters, MasterClientCharacters;
        [SerializeField] private List<GameObject> deployableList = new List<GameObject>(); // Storing deployable character information

        private List<GameObject> unDeployableList = new List<GameObject>(); // List to temporarily save deployabled characters
        private GameObject[] charArray = new GameObject[10]; // Initial storage of instantiated characters
        private List<Vector3Int> deployPositionList = new List<Vector3Int>(); // Initial storage of deployable tiles
        private int deployCounter; // Deployable character number
        private List<PickCharacter> pCharacters = new List<PickCharacter>(); // A list with character information during the game
        //private Dictionary<CID, GameObject> pCharacterObjects = new Dictionary<CID, GameObject>();
        private Dictionary<CID, Vector3Int> deployDontDestroy = new Dictionary<CID, Vector3Int>();*/


        

        [SerializeField]
        public List<CharacterBase> CharaList = new List<CharacterBase>();

        public readonly List<Vector2Int> SelectableCoords = new List<Vector2Int>(); // load from other file

        private readonly List<CID> cidList = new List<CID>();
        private readonly List<Vector2Int> nowSetableList = new List<Vector2Int>();

        private bool nowSelectable = true;

        private int selectedCid;
        private readonly string spawnableTileName = "tileWater_full";
        readonly Dictionary<CID, Vector3Int> setLocList = new Dictionary<CID, Vector3Int>();
        readonly Dictionary<CID, GameObject> setCidList = new Dictionary<CID, GameObject>();

        Color transparent = new Color(1, 1, 1, 0);

        #region Private Fields

        MouseInput mouseInput;

        #endregion


        private void SetSelectableCoords()
        {
            // 나중에 파일을 통해 불러오도록
            // 일단 하드 코딩
            if (PhotonNetwork.IsMasterClient)
            {
                SelectableCoords.Add(new Vector2Int(-1, 3));
                SelectableCoords.Add(new Vector2Int(-1, 2));
                SelectableCoords.Add(new Vector2Int(-2, 2));
                SelectableCoords.Add(new Vector2Int(-3, 1));
                SelectableCoords.Add(new Vector2Int(-2, 0));
                SelectableCoords.Add(new Vector2Int(-3, -1));
                SelectableCoords.Add(new Vector2Int(-2, -2));
                SelectableCoords.Add(new Vector2Int(-1, -2));
                SelectableCoords.Add(new Vector2Int(-1, -3));
            }
            else
            {
                SelectableCoords.Add(new Vector2Int(1, 2));
                SelectableCoords.Add(new Vector2Int(0, 3));
                SelectableCoords.Add(new Vector2Int(2, 2));
                SelectableCoords.Add(new Vector2Int(2, 1));
                SelectableCoords.Add(new Vector2Int(2, 0));
                SelectableCoords.Add(new Vector2Int(2, -1));
                SelectableCoords.Add(new Vector2Int(2, -2));
                SelectableCoords.Add(new Vector2Int(1, -2));
                SelectableCoords.Add(new Vector2Int(0, -3));
            }

            foreach(Vector2Int v in SelectableCoords)
            {
                nowSetableList.Add(v);
            }
        }

        private void SetCidList()
        {
            foreach(CharacterBase cb in CharaList)
            {
                cidList.Add(cb.cid);
            }
        }

        #region Public Methods

        public void StartControl()
        {
            mouseInput.Mouse.MouseClick.performed += OnClick;
            HighlightSelectableTile();
        }

        public void StopControl()
        {
            mouseInput.Mouse.MouseClick.performed -= OnClick;
            ClearHighlightedTile();
        }

        public void SetSelChara(CID cid)
        {
            /*PickCharacter c = GetCertainCharacter(cid);
            if (c == null)
            {
                Debug.LogErrorFormat("Can not select - cid: {0}", cid);
                return;
            }
            mouseInput.Mouse.MouseClick.performed += OnClick;
            foreach (GameObject g in unDeployableList)
            {
                if (g.name == cid.ToString())
                {
                    deployableList.Add(g);
                    unDeployableList.Remove(g);
                    break; // InvalidOperationException: Collection was modified; enumeration operation may not execute.
                }
            }
            Destroy(GameObject.Find(cid.ToString() + "(Clone)")); // Destroy clicked gameObject
            Debug.Log("Character destroyed: " + cid);
            deployPositionList.Add(deployDontDestroy[cid]); // Recover deployable tile
            deployDontDestroy.Remove(cid);
            foreach (PickCharacter ch in pCharacters)
            {
                if (ch.Cb.cid == cid)
                {
                    pCharacters.Remove(ch);
                    break;
                }
            }
            // GameObject gameObject = undeployList.Find(x => x.name.Equals(cid));
            deployCounter++;*/
        }

        public void GetDeployPosition()
        {
            /*foreach (Vector3Int position in map.cellBounds.allPositionsWithin)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (map.HasTile(position) && position.x < 0 && map.GetSprite(position).name == spawnableTileName)
                    {
                        deployPositionList.Add(position);
                    }
                }
                else
                {
                    if (map.HasTile(position) && position.x > 0 && map.GetSprite(position).name == spawnableTileName)
                    {
                        deployPositionList.Add(position);
                    }
                }
            }*/
        }

        public void OnCharaSelected(CID cid)
        {
            selectedCid = (int)cid;

            if (nowSelectable)
            {
                // 캐릭터 아이콘이 클릭되면 실행
                StartControl();
            }
        }

        public void SelectedCharaClear()
        {
            selectedCid = NullCID;
        }

        public void HighlightSelectableTile()
        {
            ClearHighlightedTile();

            foreach(Vector2Int pos in nowSetableList)
            {
                Vector3Int v = new Vector3Int(pos.x, pos.y, 0);
                if (hlMap.HasTile(v))
                {
                    hlMap.SetTileFlags(v, TileFlags.None);
                    hlMap.SetColor(v, HighlightColor);
                }
            }
        }

        public void ClearHighlightedTile()
        {
            foreach (var pos in hlMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (hlMap.HasTile(localPlace))
                {
                    hlMap.SetTileFlags(localPlace, TileFlags.None);
                    hlMap.SetColor(localPlace, transparent);
                }
            }
        }

        public void OnClickTest(InputAction.CallbackContext context) 
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider == null)
            {
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                Vector3Int cellV = map.WorldToCell(mousePosition);

                if (map.HasTile(cellV))
                {
                    Debug.Log(cellV);
                }
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (selectedCid == NullCID)
            {
                Debug.Log("Error");
                return;
            }

            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider == null) // Instantiate turns off Box Collider 2D option
            {
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                CID sCID = (CID)selectedCid;
                Vector3Int cellV = map.WorldToCell(mousePosition);
                Vector3 worldV = map.CellToWorld(cellV);
                Sprite sprite = map.GetSprite(cellV);

                if (!sprite)
                {
                    // 맵이 클릭되지 않았을 때 아무 반응 x
                    return;
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    if (mousePosition.x < 0 && map.GetSprite(cellV).name == spawnableTileName)
                    {
                        if (setLocList.ContainsValue(cellV))
                        {
                            PanelBuilder.ShowFadeOutText(Canvas.transform, "Only one character can be set on one tile");
                            StopControl();
                            return;
                        }

                        SetCharacterOnTile(sCID, cellV);

                        StopControl();
                        SelectedCharaClear();
                    }
                    else
                    {
                        PanelBuilder.ShowFadeOutText(Canvas.transform, "it is not selectable tile on your side");
                        StopControl();
                        return;
                    }
                }
                else
                {
                    if (mousePosition.x > 0 && map.GetSprite(cellV).name == spawnableTileName)
                    {
                        if (setLocList.ContainsValue(cellV))
                        {
                            PanelBuilder.ShowFadeOutText(Canvas.transform, "Only one character can be set on one tile");
                            StopControl();
                            return;
                        }

                        SetCharacterOnTile(sCID, cellV);

                        StopControl();
                        SelectedCharaClear();
                    }
                    else
                    {
                        PanelBuilder.ShowFadeOutText(Canvas.transform, "it is not selectable tile on your side");
                        StopControl();
                        return;
                    }
                }
            }
        }

        private void SetCharacterOnTile(CID cid, Vector3Int pos)
        {            
            if (setCidList.ContainsKey(cid))
            {
                Destroy(setCidList[cid]);
                nowSetableList.Add(new Vector2Int(setLocList[cid].x, setLocList[cid].y));
                setCidList.Remove(cid);
                setLocList.Remove(cid);
            }
            else
            {
                cidList.Remove(cid);
            }

            GameObject o = Instantiate(
                PickCharacterResources.LoadCharacter(cid),
                map.CellToWorld(pos),
                Quaternion.identity);
            
            if (!PhotonNetwork.IsMasterClient)
            {
                o.GetComponent<SpriteRenderer>().flipX = true;
            }

            setLocList.Add(cid, pos);
            setCidList.Add(cid, o);

            cidList.Remove(cid);
            nowSetableList.Remove(new Vector2Int(pos.x, pos.y));
        }

        public void RandomDeployCharacter()
        {
            int needToSetNum = SetableNum - setCidList.Count;

            for (int i=0; i<needToSetNum; i++)
            {
                CID cid = cidList[Random.Range(0, cidList.Count-1)];
                Vector2Int loc = nowSetableList[Random.Range(0, nowSetableList.Count - 1)];

                SetCharacterOnTile(cid, new Vector3Int(loc.x, loc.y, 0));
            }
        }

        public void StopSelect()
        {
            selectedCid = NullCID;

            mouseInput.Mouse.MouseClick.performed -= OnClick;

            nowSelectable = false;
            ClearHighlightedTile();
        }

        public void SavePickData()
        {
            Team team = PhotonNetwork.IsMasterClient ? Team.A : Team.B;

            GameObject o = new GameObject("PickData");
            PickData component = o.AddComponent<PickData>();


            foreach(CID cid in setCidList.Keys)
            {
                component.AddData(cid, setLocList[cid], team);
            }

            DontDestroyOnLoad(o);

            Debug.Log(component);
        }

        #endregion

        private void Awake()
        {
            mouseInput = new MouseInput();
        }
        private void OnEnable()
        {
            mouseInput.Enable();
        }

        private void OnDisable()
        {
            mouseInput.Disable();
        }

        private void Start()
        {
            ClearHighlightedTile();

            // 타일 좌표 알고 싶을 때 테스트 용
            //mouseInput.Mouse.MouseClick.performed += OnClickTest;

            SetSelectableCoords();
            SetCidList();
        }
    }

    class PickCharacterResources
    {
        // [Type]_[CID]
        public static string Flappy_1 = "Prefabs/Characters/PickCharacters/PickFlappy";
        public static string Flappy2_2 = "Prefabs/Characters/PickCharacters/PickFlappy2";
        public static string Knight_3 = "Prefabs/Characters/PickCharacters/PickKnight";

        public static GameObject LoadCharacter(CID cid)
        {
            return cid switch
            {
                CID.Flappy => Resources.Load<GameObject>(Flappy_1),
                CID.Flappy2 => Resources.Load<GameObject>(Flappy2_2),
                CID.Knight => Resources.Load<GameObject>(Knight_3),
                _ => null,
            };
        }

        public static string GetPathCharacter(CID cid)
        {
            return cid switch
            {
                CID.Flappy => Flappy_1,
                CID.Flappy2 => Flappy2_2,
                CID.Knight => Knight_3,
                _ => null,
            };
        }
    }
}