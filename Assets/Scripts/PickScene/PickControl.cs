using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Photon.Pun;
using System;

using KWY;

namespace PickScene
{
    public class PickControl : Singleton<PickControl>
    {
        [SerializeField] Tilemap map;
        [SerializeField] GameObject ClientCharacters, MasterClientCharacters;
        [SerializeField] private List<GameObject> deployableList = new List<GameObject>(); // Storing deployable character information

        private List<GameObject> unDeployableList = new List<GameObject>(); // List to temporarily save deployabled characters
        private GameObject[] charArray = new GameObject[10]; // Initial storage of instantiated characters
        private List<Vector3Int> deployPositionList = new List<Vector3Int>(); // Initial storage of deployable tiles
        private int deployCounter; // Deployable character number
        private List<Character> pCharacters = new List<Character>(); // A list with character information during the game
        //private Dictionary<CID, GameObject> pCharacterObjects = new Dictionary<CID, GameObject>();
        private Dictionary<CID, Vector3Int> deployDontDestroy = new Dictionary<CID, Vector3Int>();

        #region Private Fields

        MouseInput mouseInput;

        #endregion

        #region Public Methods

        public void StartControl()
        {
            mouseInput.Mouse.MouseClick.performed += OnClick;
        }

        public void SetSelClear()
        {
            mouseInput.Mouse.MouseClick.performed -= OnClick;
        }

        public void SetSelChara(CID cid)
        {
            Character c = GetCertainCharacter(cid);
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
            foreach (Character ch in pCharacters)
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

        public Character GetCertainCharacter(CID cid)
        {
            foreach (Character c in pCharacters)
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
                    if (map.HasTile(position) && position.x < 0 && map.GetSprite(position).name == "tileWater_full")
                    {
                        deployPositionList.Add(position);
                    }
                }
                else
                {
                    if (map.HasTile(position) && position.x > 0 && map.GetSprite(position).name == "tileWater_full")
                    {
                        deployPositionList.Add(position);
                    }
                }
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            // When clicking on a character
            if (hit.collider != null)
            {
                CID cid = hit.collider.gameObject.GetComponent<Character>().Cb.cid;
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
                            pCharacters.Add(charArray[prefabEnum].GetComponent<Character>());

                            unDeployableList.Add(PickManager.Instance.ClickedBtn.CharacterPrefab);
                            deployableList.Remove(PickManager.Instance.ClickedBtn.CharacterPrefab);
                            deployPositionList.Remove(clickV);
                            deployDontDestroy.Add(charArray[prefabEnum].GetComponent<Character>().Cb.cid, clickV);

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
                            pCharacters.Add(charArray[prefabEnum].GetComponent<Character>());
                            //pCharacterObjects.Add(pCharacters[deployCounter].Cb.cid, firstDeployed[deployCounter]);

                            unDeployableList.Add(PickManager.Instance.ClickedBtn.CharacterPrefab);
                            deployableList.Remove(PickManager.Instance.ClickedBtn.CharacterPrefab); // Remove from list of deployable characters
                            deployPositionList.Remove(clickV); // Remove from list of available locations
                            deployDontDestroy.Add(charArray[prefabEnum].GetComponent<Character>().Cb.cid, clickV);

                            charArray[prefabEnum].transform.parent = ClientCharacters.transform;
                            charArray[prefabEnum].GetComponent<SpriteRenderer>().flipX = true;
                            charArray[prefabEnum].GetComponent<BoxCollider2D>().enabled = true;
                            PickManager.Instance.PickClear(); // Disable button prefab
                            deployCounter--;
                        }
                    }
                }
            }
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
                pCharacters.Add(charArray[prefabEnum].GetComponent<Character>());
                //pCharacterObjects.Add(pCharacters[deployCounter].Cb.cid, firstDeployed[deployCounter]);

                charArray[prefabEnum].transform.parent = ClientCharacters.transform;
                charArray[prefabEnum].GetComponent<SpriteRenderer>().flipX = true;
                charArray[prefabEnum].GetComponent<BoxCollider2D>().enabled = true;
                deployDontDestroy.Add(charArray[prefabEnum].GetComponent<Character>().Cb.cid, Vector3Int.FloorToInt(posAdaptation)); // Store CID and location information
            }
        }

        #endregion

        #region MonoBehaviour CallBacks

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
            StartControl();
            deployCounter = 3;
            GetDeployPosition();
        }

        void Update()
        {
            //if (mouseInput.Mouse.MouseClick.IsPressed())
            //{
            //    //mouseInput.Mouse.MouseClick.performed -= CharMoveSelect;
            //    //mouseInput.Mouse.MouseClick.performed -= CharAttackSelect;
            //    //mouseInput.Mouse.MouseClick.performed += OnClick;
            //}
        }

        #endregion
    }
}
