using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Photon.Pun;
using System;
using KWY;
using UI;

namespace PickScene
{
    public class PickControl : Singleton<PickControl>
    {
        private readonly int NullCID = -1;

        [SerializeField] Tilemap map;
        [SerializeField] GameObject ClientCharacters, MasterClientCharacters;
        [SerializeField] private List<GameObject> deployableList = new List<GameObject>(); // Storing deployable character information

        private List<GameObject> unDeployableList = new List<GameObject>(); // List to temporarily save deployabled characters
        private GameObject[] charArray = new GameObject[10]; // Initial storage of instantiated characters
        private List<Vector3Int> deployPositionList = new List<Vector3Int>(); // Initial storage of deployable tiles
        private int deployCounter; // Deployable character number
        private List<PickCharacter> pCharacters = new List<PickCharacter>(); // A list with character information during the game
        //private Dictionary<CID, GameObject> pCharacterObjects = new Dictionary<CID, GameObject>();
        private Dictionary<CID, Vector3Int> deployDontDestroy = new Dictionary<CID, Vector3Int>();


        [SerializeField]
        GameObject Canvas;

        private int selectedCid;
        private readonly string spawnableTileName = "tileWater_full";
        Dictionary<CID, Vector3Int> setLocList = new Dictionary<CID, Vector3Int>();
        Dictionary<CID, GameObject> setCidList = new Dictionary<CID, GameObject>();

        #region Private Fields

        MouseInput mouseInput;

        #endregion

        #region Public Methods

        public void StartControl()
        {
            mouseInput.Mouse.MouseClick.performed += OnClick;
        }

        public void StopControl()
        {
            mouseInput.Mouse.MouseClick.performed -= OnClick;
        }

        public void SetSelChara(CID cid)
        {
            PickCharacter c = GetCertainCharacter(cid);
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
            deployCounter++;
        }

        public PickCharacter GetCertainCharacter(CID cid)
        {
            foreach (PickCharacter c in pCharacters)
            {
                if (c.Cb.cid == cid)
                {
                    return c;
                }
            }
            return null;
        }

        public void GetDeployPosition()
        {
            foreach (Vector3Int position in map.cellBounds.allPositionsWithin)
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
            }
        }

        public void OnCharaSelected(CID cid)
        {
            selectedCid = (int)cid;

            // 캐릭터 아이콘이 클릭되면 실행
            StartControl();
        }

        public void SelectedCharaClear()
        {
            selectedCid = NullCID;
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

                        if (setCidList.ContainsKey(sCID))
                        {
                            Destroy(setCidList[sCID]);
                            setCidList.Remove(sCID);
                            setLocList.Remove(sCID);
                        }

                        GameObject o = Instantiate(PickCharacterResources.LoadCharacter((CID)selectedCid), worldV, Quaternion.identity);
                        setLocList.Add(sCID, cellV);
                        setCidList.Add(sCID, o);

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

                        if (setCidList.ContainsKey(sCID))
                        {
                            Destroy(setCidList[sCID]);
                            setCidList.Remove(sCID);
                            setLocList.Remove(sCID);
                        }

                        GameObject o = Instantiate(PickCharacterResources.LoadCharacter((CID)selectedCid), worldV, Quaternion.identity);
                        o.GetComponent<SpriteRenderer>().flipX = true;
                        setLocList.Add(sCID, cellV);
                        setCidList.Add(sCID, o);

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



            /*Vector3Int clickV = map.WorldToCell(mousePosition);

            if (map.HasTile(clickV) && PickManager.Instance.ClickedBtn != null && deployableList.Contains(PickManager.Instance.ClickedBtn.CharacterPrefab))
            {
                mousePosition = map.GetCellCenterWorld(clickV); // Place only one character per tile
                mousePosition.y += (float)0.1; // Place your character in the center of a tile
                if (PhotonNetwork.IsMasterClient)
                {
                    if (mousePosition.x < 0 && map.GetSprite(clickV).name == spawnableTileName)
                    {
                        int prefabEnum = (int)Enum.Parse(typeof(CID), PickManager.Instance.ClickedBtn.CharacterPrefab.name); // To use enum as index of charArray
                        charArray[prefabEnum] = Instantiate(PickManager.Instance.ClickedBtn.CharacterPrefab, mousePosition, Quaternion.identity);
                        pCharacters.Add(charArray[prefabEnum].GetComponent<PickCharacter>());

                        unDeployableList.Add(PickManager.Instance.ClickedBtn.CharacterPrefab);
                        deployableList.Remove(PickManager.Instance.ClickedBtn.CharacterPrefab);
                        deployPositionList.Remove(clickV);
                        deployDontDestroy.Add(charArray[prefabEnum].GetComponent<PickCharacter>().Cb.cid, clickV);

                        charArray[prefabEnum].transform.parent = MasterClientCharacters.transform;
                        charArray[prefabEnum].GetComponent<BoxCollider2D>().enabled = true;
                        PickManager.Instance.PickClear();
                        deployCounter--;
                    }
                }
                else
                {
                    if (mousePosition.x > 0 && map.GetSprite(clickV).name == spawnableTileName)
                    {
                        int prefabEnum = (int)Enum.Parse(typeof(CID), PickManager.Instance.ClickedBtn.CharacterPrefab.name);
                        charArray[prefabEnum] = Instantiate(PickManager.Instance.ClickedBtn.CharacterPrefab, mousePosition, Quaternion.identity);
                        pCharacters.Add(charArray[prefabEnum].GetComponent<PickCharacter>());
                        //pCharacterObjects.Add(pCharacters[deployCounter].Cb.cid, firstDeployed[deployCounter]);

                        unDeployableList.Add(PickManager.Instance.ClickedBtn.CharacterPrefab);
                        deployableList.Remove(PickManager.Instance.ClickedBtn.CharacterPrefab); // Remove from list of deployable characters
                        deployPositionList.Remove(clickV); // Remove from list of available locations
                        deployDontDestroy.Add(charArray[prefabEnum].GetComponent<PickCharacter>().Cb.cid, clickV);

                        charArray[prefabEnum].transform.parent = ClientCharacters.transform;
                        charArray[prefabEnum].GetComponent<SpriteRenderer>().flipX = true;
                        charArray[prefabEnum].GetComponent<BoxCollider2D>().enabled = true;
                        PickManager.Instance.PickClear(); // Disable button prefab
                        deployCounter--;
                    }
                }
            }*/
            /*Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            // When clicking on a character
            if (hit.collider != null)
            {
                CID cid = hit.collider.gameObject.GetComponent<PickCharacter>().Cb.cid;
                SetSelChara(cid);
            }

            // When clicking on the map
            if (hit.collider == null) // Instantiate turns off Box Collider 2D option
            {
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector3Int clickV = map.WorldToCell(mousePosition);
                if (map.HasTile(clickV) && PickManager.Instance.ClickedBtn != null && deployableList.Contains(PickManager.Instance.ClickedBtn.CharacterPrefab))
                {
                    mousePosition = map.GetCellCenterWorld(clickV); // Place only one character per tile
                    mousePosition.y += (float)0.1; // Place your character in the center of a tile
                    if (PhotonNetwork.IsMasterClient)
                    {
                        if (mousePosition.x < 0 && map.GetSprite(clickV).name == "tileWater_full")
                        {
                            int prefabEnum = (int)Enum.Parse(typeof(CID), PickManager.Instance.ClickedBtn.CharacterPrefab.name); // To use enum as index of charArray
                            charArray[prefabEnum] = Instantiate(PickManager.Instance.ClickedBtn.CharacterPrefab, mousePosition, Quaternion.identity);
                            pCharacters.Add(charArray[prefabEnum].GetComponent<PickCharacter>());

                            unDeployableList.Add(PickManager.Instance.ClickedBtn.CharacterPrefab);
                            deployableList.Remove(PickManager.Instance.ClickedBtn.CharacterPrefab);
                            deployPositionList.Remove(clickV);
                            deployDontDestroy.Add(charArray[prefabEnum].GetComponent<PickCharacter>().Cb.cid, clickV);

                            charArray[prefabEnum].transform.parent = MasterClientCharacters.transform;
                            charArray[prefabEnum].GetComponent<BoxCollider2D>().enabled = true;
                            PickManager.Instance.PickClear();
                            deployCounter--;
                        }
                    }
                    else
                    {
                        if (mousePosition.x > 0 && map.GetSprite(clickV).name == "tileWater_full")
                        {
                            int prefabEnum = (int)Enum.Parse(typeof(CID), PickManager.Instance.ClickedBtn.CharacterPrefab.name);
                            charArray[prefabEnum] = Instantiate(PickManager.Instance.ClickedBtn.CharacterPrefab, mousePosition, Quaternion.identity);
                            pCharacters.Add(charArray[prefabEnum].GetComponent<PickCharacter>());
                            //pCharacterObjects.Add(pCharacters[deployCounter].Cb.cid, firstDeployed[deployCounter]);

                            unDeployableList.Add(PickManager.Instance.ClickedBtn.CharacterPrefab);
                            deployableList.Remove(PickManager.Instance.ClickedBtn.CharacterPrefab); // Remove from list of deployable characters
                            deployPositionList.Remove(clickV); // Remove from list of available locations
                            deployDontDestroy.Add(charArray[prefabEnum].GetComponent<PickCharacter>().Cb.cid, clickV);

                            charArray[prefabEnum].transform.parent = ClientCharacters.transform;
                            charArray[prefabEnum].GetComponent<SpriteRenderer>().flipX = true;
                            charArray[prefabEnum].GetComponent<BoxCollider2D>().enabled = true;
                            PickManager.Instance.PickClear(); // Disable button prefab
                            deployCounter--;
                        }
                    }
                }
            }*/
        }

        public void RandomDeployCharacter()
        {
            for (int i = 0; i < deployCounter; i++)
            {
                var randomPositionNum = UnityEngine.Random.Range(0, deployPositionList.Count - 1); // Picking Random Coordinates
                var randomPosition = deployPositionList[randomPositionNum];
                var posAdaptation = map.CellToWorld(randomPosition);
                posAdaptation.y += (float)0.1;
                var randomCharNum = UnityEngine.Random.Range(0, deployableList.Count - 1);
                int prefabEnum = (int)Enum.Parse(typeof(CID), deployableList[randomCharNum].name);
                charArray[prefabEnum] = Instantiate(deployableList[randomCharNum], posAdaptation, Quaternion.identity);
                deployableList.Remove(deployableList[randomCharNum]); // Remove from list of deployable characters
                deployPositionList.Remove(randomPosition); // Remove from list of available locations
                pCharacters.Add(charArray[prefabEnum].GetComponent<PickCharacter>());
                //pCharacterObjects.Add(pCharacters[deployCounter].Cb.cid, firstDeployed[deployCounter]);

                charArray[prefabEnum].transform.parent = ClientCharacters.transform;
                charArray[prefabEnum].GetComponent<SpriteRenderer>().flipX = true;
                charArray[prefabEnum].GetComponent<BoxCollider2D>().enabled = true;
                deployDontDestroy.Add(charArray[prefabEnum].GetComponent<PickCharacter>().Cb.cid, Vector3Int.FloorToInt(posAdaptation)); // Store CID and location information
            }
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
            //StartControl();
            //deployCounter = 3;
            //GetDeployPosition();
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