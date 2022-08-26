using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

using System.Collections.Generic;
using UnityEngine.EventSystems;
using Photon.Pun;

namespace KWY
{
    public class PickControl : Singleton<PickControl>
    {
        //[SerializeField] PickSceneData data;
        [SerializeField] Tilemap map;
        [SerializeField] PickManager pickManager;
        [SerializeField] TilemapControl tilemapControl;
        [SerializeField] GameObject ClientCharacters, MasterClientCharacters;
        // [SerializeField] TurnReady turnReady;

        public GameObject[] character; // �ӽ� prefabs
        private int deployCounter; // ��ġ�� ĳ���� ��
        [SerializeField] private GameObject[] pCharas = new GameObject[3];
        private GameObject[] firstDeployed = new GameObject[3]; // ��ġ�� ĳ���� ���� ����

        private List<Character> pCharacters = new List<Character>(); // ���� ���� �� ĳ���� ������ ������ �ִ� ����Ʈ
        private Dictionary<CID, GameObject> pCharacterObjects = new Dictionary<CID, GameObject>();

        private Dictionary<CID, Vector3Int> deployRPC = new Dictionary<CID, Vector3Int>();

        public Dictionary<Vector3Int, GameObject> characters = new Dictionary<Vector3Int, GameObject>();
        public List<Vector3Int> deployablePosition = new List<Vector3Int>();
        private List<int> randomNumList = new List<int>();

        //public Character SelChara { get; private set; }
        //public ActionBase SelAction { get; private set; }

        #region Private Fields

        MouseInput mouseInput;
        private int SelOk = 0; // ����: left, ���: right -> |2| �� �Ǿ��� �� �׼� Ȯ��

        #endregion

        #region Public Methods

        //public void SetSelSkill(SID sid)
        //{
        //    SelAction = SkillManager.GetData(sid);
        //}

        //public void SetSelSkill(SkillBase sb)
        //{
        //    SelAction = sb;

        //    //highLighter.HighlightMap(SelChara.TempTilePos, SelChara.TempTilePos.y % 2 == 0 ? sb.areaEvenY : sb.areaOddY);

        //}

        public void StartControl()
        {
            mouseInput.Mouse.MouseClick.performed += OnClick;
        }

        public void SetSelClear()
        {
            //SelChara = null;
            //SelAction = null;
            SelOk = 0;

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
            //showingSkillManager.ShowSkillPanel(data.GetCharacterNth(cid));
            HighlightCharacter(cid);
            //highLighter.ClearHighlight();

            mouseInput.Mouse.MouseClick.performed += OnClick;
            //mouseInput.Mouse.MouseClick.performed -= OnClickSkillDirection;
            //mouseInput.Mouse.MouseClick.performed -= OnClickMoveDirection;

            Debug.Log("Character selected: " + cid);
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
                        //Vector3 posAdaptation = map.GetCellCenterWorld(position); // Ÿ�� �� ĭ�� �� ĳ���͸� ��ġ
                        //posAdaptation.y += (float)0.1; // Ÿ�� �� ĭ�� �߾ӿ� ĳ���� ��ġ
                        deployablePosition.Add(position); // Vector3Int.FloorToInt(posAdaptation)
                    }
                }
                else
                {
                    if (map.HasTile(position) && position.x > 0 && map.GetSprite(position).name == "tileWater_full")
                    {
                        //Vector3 posAdaptation = map.GetCellCenterWorld(position); // Ÿ�� �� ĭ�� �� ĳ���͸� ��ġ
                        //posAdaptation.y += (float)0.1; // Ÿ�� �� ĭ�� �߾ӿ� ĳ���� ��ġ
                        deployablePosition.Add(position); // Vector3Int.FloorToInt(posAdaptation)
                    }
                }
            }
            //foreach (Vector3Int pos in deployPosition)
            //{
            //    Debug.Log(pos);
            //}
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                CID cid = hit.collider.gameObject.GetComponent<Character>().Cb.cid;
                SetSelChara(cid);

                // Test
                //Destroy(hit.collider.gameObject);
                //pCharacterObjects.Remove(pCharacters[deployCounter].Cb.cid);
                //deployCounter--;
            }
            if (hit.collider == null) // Instantiate�ϸ� Box Collider 2D �ɼ��� ���� ����
            {
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector3Int clickV = map.WorldToCell(mousePosition);
                bool keyExists = deployRPC.ContainsValue(clickV);
                if (PickManager.Instance.ClickedBtn != null && map.HasTile(clickV) && keyExists == false && deployCounter < 3)
                {
                    mousePosition = map.GetCellCenterWorld(clickV); // Ÿ�� �� ĭ�� �� ĳ���͸� ��ġ
                    mousePosition.y += (float)0.1; // Ÿ�� �� ĭ�� �߾ӿ� ĳ���� ��ġ
                    if (PhotonNetwork.IsMasterClient)
                    {
                        if (mousePosition.x < 0 && map.GetSprite(clickV).name == "tileWater_full")
                        {
                            firstDeployed[deployCounter] = Instantiate(PickManager.Instance.ClickedBtn.CharacterPrefab, mousePosition, Quaternion.identity);
                            pCharacters.Add(firstDeployed[deployCounter].GetComponent<Character>());
                            pCharacterObjects.Add(pCharacters[deployCounter].Cb.cid, firstDeployed[deployCounter]);

                            firstDeployed[deployCounter].transform.parent = MasterClientCharacters.transform;
                            firstDeployed[deployCounter].GetComponent<BoxCollider2D>().enabled = true;
                            PickManager.Instance.PickClear(); // ��ư Prefab ����
                            // characters.Add(clickV, firstDeployed[deployCounter]);
                            deployRPC.Add(pCharacters[deployCounter].Cb.cid, clickV); // CID�� ��ġ ���� ����
                            deployCounter++;
                        }
                    }
                    else
                    {
                        if (mousePosition.x > 0 && map.GetSprite(clickV).name == "tileWater_full")
                        {
                            firstDeployed[deployCounter] = Instantiate(PickManager.Instance.ClickedBtn.CharacterPrefab, mousePosition, Quaternion.identity);
                            pCharacters.Add(firstDeployed[deployCounter].GetComponent<Character>());
                            pCharacterObjects.Add(pCharacters[deployCounter].Cb.cid, firstDeployed[deployCounter]);

                            firstDeployed[deployCounter].transform.parent = ClientCharacters.transform;
                            firstDeployed[deployCounter].GetComponent<SpriteRenderer>().flipX = true;
                            firstDeployed[deployCounter].GetComponent<BoxCollider2D>().enabled = true;
                            PickManager.Instance.PickClear(); // ��ư Prefab ����
                            deployRPC.Add(pCharacters[deployCounter].Cb.cid, clickV); // CID�� ��ġ ���� ����
                            deployCounter++;

                        }
                    }
                    //Debug.Log(pickManager.ClickedBtn.CharacterPrefab.name + " placed at " + mousePosition);
                    
                    
                }
            }
        }

        public void RandomDeployCharacter()
        {
            if (deployCounter == 0)
            {
                for (int i = 4; i >= deployCounter; i--)
                {
                    var randomNum = Random.Range(0, deployablePosition.Count - 1);
                    randomNumList.Add(randomNum);
                    if (randomNumList.Contains(randomNum))
                    {
                        randomNum = Random.Range(0, deployablePosition.Count - 1);
                    }
                    var randomPosition = deployablePosition[randomNum];
                    bool keyCheck = deployRPC.ContainsValue(randomPosition);
                    if (keyCheck == false)
                    {
                        var posAdaptation = map.CellToWorld(randomPosition);
                        posAdaptation.y += (float)0.1;

                        var randomNum2 = Random.Range(0, character.Length - 1);
                        if (pCharacterObjects.ContainsKey(character[randomNum2].GetComponent<Character>().Cb.cid))
                        {
                            
                        }
                        firstDeployed[deployCounter] = Instantiate(character[randomNum2], posAdaptation, Quaternion.identity); // Vector3Int.FloorToInt(posAdaptation)
                        pCharacters.Add(firstDeployed[deployCounter].GetComponent<Character>());
                        pCharacterObjects.Add(pCharacters[deployCounter].Cb.cid, firstDeployed[deployCounter]);
                        


                        firstDeployed[deployCounter].transform.parent = ClientCharacters.transform;
                        firstDeployed[deployCounter].GetComponent<SpriteRenderer>().flipX = true;
                        firstDeployed[deployCounter].GetComponent<BoxCollider2D>().enabled = true;
                        deployRPC.Add(pCharacters[deployCounter].Cb.cid, Vector3Int.FloorToInt(posAdaptation)); // CID�� ��ġ ���� ����
                        deployCounter++;
                    }
                }
            }
            if (0 < deployCounter && deployCounter < 3)
            {
                for (int i = 4; i > deployCounter; i--)
                {
                    var randomNum = Random.Range(0, deployablePosition.Count - 1);
                    randomNumList.Add(randomNum);
                    while (randomNumList.Contains(randomNum))
                    {
                        randomNum = Random.Range(0, deployablePosition.Count - 1);
                    }
                    var randomPosition = deployablePosition[randomNum];
                    bool keyCheck = deployRPC.ContainsValue(randomPosition);
                    if (keyCheck == false)
                    {
                        var posAdaptation = map.CellToWorld(randomPosition);
                        posAdaptation.y += (float)0.1;
                        var randomNum2 = Random.Range(0, character.Length - 1);
                        
                        firstDeployed[deployCounter] = Instantiate(character[randomNum2], posAdaptation, Quaternion.identity); // Vector3Int.FloorToInt(posAdaptation)
                        pCharacters.Add(firstDeployed[deployCounter].GetComponent<Character>());
                        pCharacterObjects.Add(pCharacters[deployCounter].Cb.cid, firstDeployed[deployCounter]);

                        firstDeployed[deployCounter].transform.parent = ClientCharacters.transform;
                        firstDeployed[deployCounter].GetComponent<SpriteRenderer>().flipX = true;
                        firstDeployed[deployCounter].GetComponent<BoxCollider2D>().enabled = true;
                        deployRPC.Add(pCharacters[deployCounter].Cb.cid, Vector3Int.FloorToInt(posAdaptation)); // CID�� ��ġ ���� ����
                        deployCounter++;
                    }
                }
            }
            //foreach (CID cid in deployRPC.Keys)
            //{
            //    Debug.Log(deployRPC.Keys + ", " + deployRPC.Values);
            //}
            
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
            deployCounter = 0;
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
