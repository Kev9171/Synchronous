using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

using System.Collections.Generic;
using UnityEngine.EventSystems;
using Photon.Pun;
using System;

namespace KWY
{
    public class PickControl : Singleton<PickControl>
    {
        [SerializeField] Tilemap map;
        [SerializeField] GameObject ClientCharacters, MasterClientCharacters;
        [SerializeField] private List<GameObject> deployableList = new List<GameObject>(); // ��ġ ������ ĳ���� ���� ����

        private int deployCounter; // ��ġ ���� ĳ���� ��(3)
        public GameObject[] charArray = new GameObject[10]; // ��ġ�ϴ� ĳ���� ���� ����
        public List<Vector3Int> deployPositionList = new List<Vector3Int>(); // ��ġ ������ Ÿ�ϸ� ��ǥ ����

        private List<Character> pCharacters = new List<Character>(); // ���� ���� �� ĳ���� ������ ������ �ִ� ����Ʈ
        private Dictionary<CID, GameObject> pCharacterObjects = new Dictionary<CID, GameObject>();
        private Dictionary<CID, Vector3Int> deployRPC = new Dictionary<CID, Vector3Int>();

        #region Private Fields

        MouseInput mouseInput;
        //private int SelOk = 0; // ����: left, ���: right -> |2| �� �Ǿ��� �� �׼� Ȯ��

        #endregion

        #region Public Methods

        public void StartControl()
        {
            mouseInput.Mouse.MouseClick.performed += OnClick;
        }

        public void SetSelClear()
        {
            //SelChara = null;
            //SelAction = null;
            //SelOk = 0;

            mouseInput.Mouse.MouseClick.performed -= OnClick;
            //mouseInput.Mouse.MouseClick.performed -= OnClickSkillDirection;

            //showingSkillManager.ShowSkillPanel(-1);

            //highLighter.ClearHighlight();
            HighlightCharacterClear();
        }

        public void SetSelChara(CID cid)
        {
            //SelChara = null;
            Character c = GetCertainCharacter(cid);

            if (c == null)
            {
                Debug.LogErrorFormat("Can not select - cid: {0}", cid);
                return;
            }

            //SelAction = null;
            //SelChara = c;
            HighlightCharacter(cid);
            //highLighter.ClearHighlight();

            mouseInput.Mouse.MouseClick.performed += OnClick;
            Debug.Log("Character selected: " + cid);
            //CID cd = (CID)1;
            //Debug.Log((CID)"Flappy");
        }

        public void HighlightCharacterClear()
        {
            foreach (CID c in pCharacterObjects.Keys)
            {
                pCharacterObjects[c].transform.localScale = new Vector3(0.7f, 0.7f, 1);
            }
        }

        public void HighlightCharacter(CID cid)
        {
            foreach (CID c in pCharacterObjects.Keys)
            {
                if (c == cid)
                {
                    pCharacterObjects[c].transform.localScale = new Vector3(1, 1, 1);
                    //Destroy(pCharacterObjects[c]);
                    
                }
                else
                {
                    pCharacterObjects[c].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                }

            }

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

        public void UndeployCharacter(CID cid)
        {

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

            // ĳ���� Ŭ�� ��
            if (hit.collider != null)
            {
                CID cid = hit.collider.gameObject.GetComponent<Character>().Cb.cid;
                SetSelChara(cid);
            }

            // �� Ŭ�� ��
            if (hit.collider == null) // Instantiate�ϸ� Box Collider 2D �ɼ��� ���� ����
            {
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector3Int clickV = map.WorldToCell(mousePosition);
                //bool keyExists = deployRPC.ContainsValue(clickV);
                
                if (map.HasTile(clickV) && PickManager.Instance.ClickedBtn != null && deployableList.Contains(PickManager.Instance.ClickedBtn.CharacterPrefab)) // keyExists == false && 
                {
                    mousePosition = map.GetCellCenterWorld(clickV); // Ÿ�� �� ĭ�� �� ĳ���͸� ��ġ
                    mousePosition.y += (float)0.1; // Ÿ�� �� ĭ�� �߾ӿ� ĳ���� ��ġ
                    if (PhotonNetwork.IsMasterClient)
                    {
                        if (mousePosition.x < 0 && map.GetSprite(clickV).name == "tileWater_full")
                        {
                            int prefabEnum = (int)Enum.Parse(typeof(CID), PickManager.Instance.ClickedBtn.CharacterPrefab.name); // enum�� charArray�� index�� �̿��ϱ� ����
                            charArray[prefabEnum] = Instantiate(PickManager.Instance.ClickedBtn.CharacterPrefab, mousePosition, Quaternion.identity);
                            pCharacters.Add(charArray[deployCounter].GetComponent<Character>());
                            //pCharacterObjects.Add(pCharacters[deployCounter].Cb.cid, charArray[deployCounter]);
                            deployableList.Remove(PickManager.Instance.ClickedBtn.CharacterPrefab);
                            deployPositionList.Remove(clickV);

                            charArray[prefabEnum].transform.parent = MasterClientCharacters.transform;
                            charArray[prefabEnum].GetComponent<BoxCollider2D>().enabled = true;
                            PickManager.Instance.PickClear();
                            //deployRPC.Add(pCharacters[deployCounter].Cb.cid, clickV); // CID�� ��ġ ���� ����
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
                            deployableList.Remove(PickManager.Instance.ClickedBtn.CharacterPrefab); // ��ġ ������ ĳ���� list���� ����
                            deployPositionList.Remove(clickV); // ��ġ ������ ��ġ list���� ����

                            charArray[prefabEnum].transform.parent = ClientCharacters.transform;
                            charArray[prefabEnum].GetComponent<SpriteRenderer>().flipX = true;
                            charArray[prefabEnum].GetComponent<BoxCollider2D>().enabled = true;
                            PickManager.Instance.PickClear(); // ��ư Prefab ����
                            //deployRPC.Add(pCharacters[deployCounter].Cb.cid, clickV); // CID�� ��ġ ���� ����
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
                var randomPositionNum = UnityEngine.Random.Range(0, deployPositionList.Count - 1); // Random ��ǥ ����
                var randomPosition = deployPositionList[randomPositionNum]; // bool keyCheck = deployRPC.ContainsValue(randomPosition);
                var posAdaptation = map.CellToWorld(randomPosition);
                posAdaptation.y += (float)0.1;
                var randomCharNum = UnityEngine.Random.Range(0, deployableList.Count - 1);
                int prefabEnum = (int)Enum.Parse(typeof(CID), deployableList[randomCharNum].name);
                charArray[prefabEnum] = Instantiate(deployableList[randomCharNum], posAdaptation, Quaternion.identity); // Vector3Int.FloorToInt(posAdaptation)
                deployableList.Remove(deployableList[randomCharNum]); // ��ġ ������ ĳ���� list���� ����
                deployPositionList.Remove(randomPosition); // ��ġ ������ ��ġ list���� ����
                pCharacters.Add(charArray[prefabEnum].GetComponent<Character>());
                //pCharacterObjects.Add(pCharacters[deployCounter].Cb.cid, firstDeployed[deployCounter]);

                charArray[prefabEnum].transform.parent = ClientCharacters.transform;
                charArray[prefabEnum].GetComponent<SpriteRenderer>().flipX = true;
                charArray[prefabEnum].GetComponent<BoxCollider2D>().enabled = true;
                //deployRPC.Add(pCharacters[deployCounter].Cb.cid, Vector3Int.FloorToInt(posAdaptation)); // CID�� ��ġ ���� ����
            }
        }

        #endregion


        #region Private Methods

        void CharMoveSelect(InputAction.CallbackContext context)
        {
            /*Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            CharacterActionData cad = data.GetActionData(SelChara.Cb.cid);

            if (map.HasTile(gridPosition) && cad.Count < 3)
            {
                Vector2 deltaXY = ((Vector2Int)gridPosition) - SelChara.TempTilePos;
                object[] arr = { "move", deltaXY.x, deltaXY.y };
                
                cad.AddAction(ActionType.Move, deltaXY.x, deltaXY.y);
                // selChara.SetTilePos((Vector2Int)gridPosition); �ʿ�?
                Debug.Log("tile selected");
            }
            else if (!map.HasTile(gridPosition))
                Debug.Log("no tile selected");
            else
            {
                Debug.Log("�� �̻� �߰� �Ұ�");
                foreach (KeyValuePair<int, object[]> item in cad.Actions)
                {
                    Debug.Log(item.Key + ", " + item.Value[0] + ", " + item.Value[1] + ", " + item.Value[2]);
                }
            }*/
        }

        void CharAttackSelect(InputAction.CallbackContext context)
        {
            /*Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            CharacterActionData cad = data.GetActionData(SelChara.Cb.cid);

            Vector2 deltaXY = ((Vector2Int)gridPosition) - SelChara.TempTilePos;
            if (map.HasTile(gridPosition) && deltaXY.sqrMagnitude <= 2 && cad.Count < 3)
            {
                object[] arr = { "skill", deltaXY.x, deltaXY.y };
                cad.AddAction(ActionType.Skill, SID.FireBall, SkillDicection.Left); // temp
                Debug.Log("attack selected");
            }
            else if (deltaXY.sqrMagnitude > 2)
                Debug.Log("outside the atk range");
            else
            {
                Debug.Log("�� �̻� �߰� �Ұ�");
                foreach (KeyValuePair<int, object[]> item in cad.Actions)
                {
                    Debug.Log(item.Key + ", " + item.Value[0] + ", " + item.Value[1] + ", " + item.Value[2]);
                }
            }*/
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
