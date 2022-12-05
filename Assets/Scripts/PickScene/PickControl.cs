using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Photon.Pun;
using System;
using TMPro;
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
        [SerializeField] Button removeCharacterBtn;
        [SerializeField] GameObject characterBtnsParentObject;
        [SerializeField] TMP_Text removeCharacterLabel;
        [SerializeField] TMP_Text setCharacterLabel;

        [SerializeField]
        GameObject btnPrefab;

        [Tooltip("배치 가능한 캐릭터 리스트")]
        [SerializeField]
        List<CharacterBase> CharaList = new List<CharacterBase>();

        /// <summary>
        /// 배치 가능한 타일들의 좌표 리스트 (하드 코딩되어있음)
        /// </summary>
        private readonly List<Vector2Int> SelectableCoords = new List<Vector2Int>(); // load from other file

        /// <summary>
        /// 현재 배치 되지 않아서 배치 될 수 있는 캐릭터들의 cid 리스트
        /// </summary>
        private readonly List<CID> cidList = new List<CID>();

        /// <summary>
        /// 현재 배치 가능한 타일들의 좌표 리스트
        /// </summary>
        private readonly List<Vector2Int> nowSetableList = new List<Vector2Int>();

        /// <summary>
        /// 배치하거나 취소할 수 있는 상태
        /// </summary>
        private bool nowSelectable = true;

        /// <summary>
        /// 현재 선택되어 있는 캐릭터의 cid (선택되어 있는 캐릭터가 없으면 -1)
        /// </summary>
        private int selectedCid;

        private readonly string spawnableTileName = "tileWater_full";

        /// <summary>
        /// 현재 배치되어 있는 캐릭터(key)와 그 캐릭터가 배치되어있는 파일 맵의 좌표(value)
        /// </summary>
        readonly Dictionary<CID, Vector3Int> setLocList = new Dictionary<CID, Vector3Int>();

        /// <summary>
        /// 현재 배치되어 있는 캐릭터(key)와 그 캐릭터의 실제 GameObject(value)
        /// </summary>
        readonly Dictionary<CID, GameObject> setCidList = new Dictionary<CID, GameObject>();

        readonly Color transparent = new Color(1, 1, 1, 0);

        MouseInput mouseInput;


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

        #region Public Methods

        public void StartControlSet()
        {
            setCharacterLabel.gameObject.SetActive(true);
            mouseInput.Mouse.MouseClick.performed += OnClickSetCharacter;
            HighlightSelectableTile();
        }

        public void StopControlSet()
        {
            setCharacterLabel.gameObject.SetActive(false);
            mouseInput.Mouse.MouseClick.performed -= OnClickSetCharacter;
            ClearHighlightedTile();
        }

        public void StartControlRemove()
        {
            removeCharacterLabel.gameObject.SetActive(true);

            // 캐릭터 하이라이트 되도록
            foreach(GameObject o in setCidList.Values)
            {
                o.GetComponent<PickCharacter>().Highlightable = true;
            }

            mouseInput.Mouse.MouseClick.performed += OnClickRemoveCharacter;
            ClearHighlightedTile();
        }

        public void StopControlRemove()
        {
            removeCharacterLabel.gameObject.SetActive(false);

            // 캐릭터 하이라이트 안되도록
            foreach (GameObject o in setCidList.Values)
            {
                o.GetComponent<PickCharacter>().Highlightable = false;
            }

            mouseInput.Mouse.MouseClick.performed -= OnClickRemoveCharacter;
        }

        public void OnCharaSelected(CID cid)
        {
            // 이미 배치되어 있는 캐릭터의 좌표를 바꾸는 거는 ok
            // 이미 최대 수의 캐릭터가 배치 되어 있을 경우, 배치된 캐릭터를 삭제하라는 문구 출력
            if (!setCidList.ContainsKey(cid) && setCidList.Keys.Count >= SetableNum)
            {
                PanelBuilder.ShowFadeOutText(Canvas.transform, "Settable Number of characters is 3.  Remove other character first.");
                return;
            }

            selectedCid = (int)cid;

            // 캐릭터 아이콘이 클릭되면 실행
            StartControlSet();
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

        public void OnClickSetCharacter(InputAction.CallbackContext context)
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
                //Vector3 worldV = map.CellToWorld(cellV);
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
                            StopControlSet();
                            return;
                        }

                        SetCharacterOnTile(sCID, cellV);

                        StopControlSet();
                        SelectedCharaClear();
                    }
                    else
                    {
                        PanelBuilder.ShowFadeOutText(Canvas.transform, "it is not selectable tile on your side");
                        StopControlSet();
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
                            StopControlSet();
                            return;
                        }

                        SetCharacterOnTile(sCID, cellV);

                        StopControlSet();
                        SelectedCharaClear();
                    }
                    else
                    {
                        PanelBuilder.ShowFadeOutText(Canvas.transform, "it is not selectable tile on your side");
                        StopControlSet();
                        return;
                    }
                }
            }
        }

        public void OnClickRemoveCharacter(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null) // Instantiate turns off Box Collider 2D option
            {
                if (hit.collider.gameObject.TryGetComponent<PickCharacter>(out var chara))
                {
                    // remove character
                    RemoveCharacterOnTile(chara.Cb.cid);
                }
                else
                {
                    // 다른 곳 클릭시 canel
                    
                }
            }
            else
            {
                
            }

            StopControlRemove();
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

        private void RemoveCharacterOnTile(CID cid)
        {
            // cid가 이미 배치되어 있는 캐릭터인지 확인 (반드시 true)
            if(!setLocList.ContainsKey(cid) || !setCidList.ContainsKey(cid))
            {
                Debug.LogError("ERROR  at RemoveCharacterOnTile");
                return;
            }

            // 게임 오브젝트 destroy
            Destroy(setCidList[cid]);

            // 해당 위치에 다시 배치가 가능해졌으므로 배치 가능한 타일 좌표에 다시 추가
            nowSetableList.Add(new Vector2Int(setLocList[cid].x, setLocList[cid].y));

            // 각 리스트에서 제거
            setCidList.Remove(cid);
            setLocList.Remove(cid);

            // 배치 될 수 있는 cid 리스트에 추가
            cidList.Add(cid);
        }

        private void CharacterBtnOnClicked(CID cid)
        {
            if (!nowSelectable)
            {
                return;
            }

            if (setCidList.ContainsKey(cid))
            {
                PanelBuilder.ShowFadeOutText(Canvas.transform, "Choose a new tile to change the position");
            }
            OnCharaSelected(cid);
        }

        private void RemoveCharacterBtnOnClicked()
        {
            // 배치되어 있는 캐릭터가 없으면 x
            if(setCidList.Keys.Count == 0)
            {
                PanelBuilder.ShowFadeOutText(Canvas.transform, "Set the character at tile at least one first");
                return;
            }

            selectedCid = NullCID;

            mouseInput.Mouse.MouseClick.performed -= OnClickSetCharacter;
            ClearHighlightedTile();

            selectedCid = NullCID;

            GameObject eventSystem = GameObject.Find("EventSystem");
            eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);

            StartControlRemove();
        }

        public void RandomDeployCharacter()
        {
            int needToSetNum = SetableNum - setCidList.Count;

            for (int i=0; i<needToSetNum; i++)
            {
                CID cid = cidList[Random.Range(0, cidList.Count)];
                Vector2Int loc = nowSetableList[Random.Range(0, nowSetableList.Count)];

                SetCharacterOnTile(cid, new Vector3Int(loc.x, loc.y, 0));
            }
        }

        public void StopSelect()
        {
            selectedCid = NullCID;

            mouseInput.Mouse.MouseClick.performed -= OnClickSetCharacter;
            mouseInput.Mouse.MouseClick.performed -= OnClickRemoveCharacter;

            removeCharacterBtn.gameObject.SetActive(false);

            foreach(Button o in characterBtnsParentObject.GetComponentsInChildren<Button>())
            {
                o.interactable = false;
            }

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
            setCharacterLabel.gameObject.SetActive(false);
            removeCharacterLabel.gameObject.SetActive(false);

            // 타일 좌표 알고 싶을 때 테스트 용
            //mouseInput.Mouse.MouseClick.performed += OnClickTest;

            SetSelectableCoords();

            foreach (CharacterBase cb in CharaList)
            {
                cidList.Add(cb.cid);
            }

            foreach (CharacterBase cb in CharaList)
            {
                GameObject o = Instantiate(btnPrefab);
                o.GetComponent<CharacterBtn>().Init(cb.icon, cb.cid);
                o.GetComponent<Button>().onClick.AddListener(delegate { CharacterBtnOnClicked(cb.cid); });
                o.transform.SetParent(characterBtnsParentObject.transform);
            }

            removeCharacterBtn.onClick.AddListener(RemoveCharacterBtnOnClicked);
        }
    }

    class PickCharacterResources
    {
        // [Type]_[CID]
        public static string Flappy_1 = "Prefabs/Characters/PickCharacters/PickFlappy";
        public static string Flappy2_2 = "Prefabs/Characters/PickCharacters/PickFlappy2";
        public static string Knight_3 = "Prefabs/Characters/PickCharacters/PickKnight";
        public static string Spearman_4 = "Prefabs/Characters/PickCharacters/PickSpearman";
        public static string Healer_5 = "Prefabs/Characters/PickCharacters/PickHealer";

        public static GameObject LoadCharacter(CID cid)
        {
            return cid switch
            {
                CID.Flappy => Resources.Load<GameObject>(Flappy_1),
                CID.Flappy2 => Resources.Load<GameObject>(Flappy2_2),
                CID.Knight => Resources.Load<GameObject>(Knight_3),
                CID.Spearman => Resources.Load<GameObject>(Spearman_4),
                CID.Healer => Resources.Load<GameObject>(Healer_5),
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
                CID.Spearman => Spearman_4,
                CID.Healer => Healer_5,
                _ => null,
            };
        }
    }
}