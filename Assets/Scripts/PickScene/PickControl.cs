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

        [Tooltip("��ġ ������ ĳ���� ����Ʈ")]
        [SerializeField]
        List<CharacterBase> CharaList = new List<CharacterBase>();

        /// <summary>
        /// ��ġ ������ Ÿ�ϵ��� ��ǥ ����Ʈ (�ϵ� �ڵ��Ǿ�����)
        /// </summary>
        private readonly List<Vector2Int> SelectableCoords = new List<Vector2Int>(); // load from other file

        /// <summary>
        /// ���� ��ġ ���� �ʾƼ� ��ġ �� �� �ִ� ĳ���͵��� cid ����Ʈ
        /// </summary>
        private readonly List<CID> cidList = new List<CID>();

        /// <summary>
        /// ���� ��ġ ������ Ÿ�ϵ��� ��ǥ ����Ʈ
        /// </summary>
        private readonly List<Vector2Int> nowSetableList = new List<Vector2Int>();

        /// <summary>
        /// ��ġ�ϰų� ����� �� �ִ� ����
        /// </summary>
        private bool nowSelectable = true;

        /// <summary>
        /// ���� ���õǾ� �ִ� ĳ������ cid (���õǾ� �ִ� ĳ���Ͱ� ������ -1)
        /// </summary>
        private int selectedCid;

        private readonly string spawnableTileName = "tileWater_full";

        /// <summary>
        /// ���� ��ġ�Ǿ� �ִ� ĳ����(key)�� �� ĳ���Ͱ� ��ġ�Ǿ��ִ� ���� ���� ��ǥ(value)
        /// </summary>
        readonly Dictionary<CID, Vector3Int> setLocList = new Dictionary<CID, Vector3Int>();

        /// <summary>
        /// ���� ��ġ�Ǿ� �ִ� ĳ����(key)�� �� ĳ������ ���� GameObject(value)
        /// </summary>
        readonly Dictionary<CID, GameObject> setCidList = new Dictionary<CID, GameObject>();

        readonly Color transparent = new Color(1, 1, 1, 0);

        MouseInput mouseInput;


        private void SetSelectableCoords()
        {
            // ���߿� ������ ���� �ҷ�������
            // �ϴ� �ϵ� �ڵ�
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

            // ĳ���� ���̶���Ʈ �ǵ���
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

            // ĳ���� ���̶���Ʈ �ȵǵ���
            foreach (GameObject o in setCidList.Values)
            {
                o.GetComponent<PickCharacter>().Highlightable = false;
            }

            mouseInput.Mouse.MouseClick.performed -= OnClickRemoveCharacter;
        }

        public void OnCharaSelected(CID cid)
        {
            // �̹� ��ġ�Ǿ� �ִ� ĳ������ ��ǥ�� �ٲٴ� �Ŵ� ok
            // �̹� �ִ� ���� ĳ���Ͱ� ��ġ �Ǿ� ���� ���, ��ġ�� ĳ���͸� �����϶�� ���� ���
            if (!setCidList.ContainsKey(cid) && setCidList.Keys.Count >= SetableNum)
            {
                PanelBuilder.ShowFadeOutText(Canvas.transform, "Settable Number of characters is 3.  Remove other character first.");
                return;
            }

            selectedCid = (int)cid;

            // ĳ���� �������� Ŭ���Ǹ� ����
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
                    // ���� Ŭ������ �ʾ��� �� �ƹ� ���� x
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
                    // �ٸ� �� Ŭ���� canel
                    
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
            // cid�� �̹� ��ġ�Ǿ� �ִ� ĳ�������� Ȯ�� (�ݵ�� true)
            if(!setLocList.ContainsKey(cid) || !setCidList.ContainsKey(cid))
            {
                Debug.LogError("ERROR  at RemoveCharacterOnTile");
                return;
            }

            // ���� ������Ʈ destroy
            Destroy(setCidList[cid]);

            // �ش� ��ġ�� �ٽ� ��ġ�� �����������Ƿ� ��ġ ������ Ÿ�� ��ǥ�� �ٽ� �߰�
            nowSetableList.Add(new Vector2Int(setLocList[cid].x, setLocList[cid].y));

            // �� ����Ʈ���� ����
            setCidList.Remove(cid);
            setLocList.Remove(cid);

            // ��ġ �� �� �ִ� cid ����Ʈ�� �߰�
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
            // ��ġ�Ǿ� �ִ� ĳ���Ͱ� ������ x
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

            // Ÿ�� ��ǥ �˰� ���� �� �׽�Ʈ ��
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