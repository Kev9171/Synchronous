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
        [SerializeField] PickSceneData data;
        [SerializeField] Tilemap map;
        [SerializeField] PickManager pickManager;
        [SerializeField] TilemapControl tilemapControl;
        [SerializeField] GameObject ClientCharacters, MasterClientCharacters;
        // [SerializeField] TurnReady turnReady;

        public GameObject[] character;
        public int deployCounter;
        public GameObject[] newCharacters = new GameObject[3];
        public Dictionary<Vector3Int, GameObject> characters = new Dictionary<Vector3Int, GameObject>();
        public List<Vector3Int> deployPosition = new List<Vector3Int>();
        private List<int> randomNumList = new List<int>();

        public Character SelChara { get; private set; }
        public ActionBase SelAction { get; private set; }

        #region Private Fields

        MouseInput mouseInput;
        private int SelOk = 0; // 음수: left, 양수: right -> |2| 가 되었을 때 액션 확정

        #endregion

        #region Public Methods

        //public void OnChooseSkillDirectionMode()
        //{
        //    // 캐릭터 선택 후 맵에 있는 캐릭터 클릭으로 캐릭터 선택할 수 있는 거 잠시 빼기
        //    mouseInput.Mouse.MouseClick.performed -= OnClick;

        //    // 방향 정할 수 있는 이벤트 넣기
        //    mouseInput.Mouse.MouseClick.performed += OnClickSkillDirection;
        //}

        //public void OnChooseMoveDirectionmode()
        //{
        //    // 캐릭터 선택 후 맵에 있는 캐릭터 클릭으로 캐릭터 선택할 수 있는 거 잠시 빼기
        //    mouseInput.Mouse.MouseClick.performed -= OnClick;

        //    // 방향 정할 수 있는 이벤트 넣기
        //    mouseInput.Mouse.MouseClick.performed += OnClickMoveDirection;
        //}

        //public void OnClickMoveDirection(InputAction.CallbackContext context)
        //{
        //    if (SelChara == null || !(SelAction is MoveBase)) return;

        //    Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();

        //    // 바로 WorldToCell 함수에 집어넣지 말것! (???)
        //    mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //    // 사용 x
        //    // Vector3Int clickV = map.WorldToCell(mouseInput.Mouse.MouseClick.performed += OnClickMoveDirection;);

        //    // 클릭 된 좌표 맵 좌표로 변환
        //    Vector3Int clickV = map.WorldToCell(mousePosition);
        //    Vector3Int charaV = SelChara.TempTilePos;

        //    Vector2Int deltaXY = (Vector2Int)clickV - (Vector2Int)charaV;

        //    if (map.HasTile(clickV) && (SelChara.TempTilePos.y % 2 == 0 ? SelAction.areaEvenY : SelAction.areaOddY).Contains(deltaXY))
        //    {
        //        data.CharaActionData[SelChara.Cb.cid].AddMoveAction(ActionType.Move, (int)deltaXY.x, (int)deltaXY.y, SelChara.TempTilePos.y%2!=0);

        //        // 이동 넣었을 경우 하이라이트를 위한 임시 좌표 변경
        //        SelChara.SetTilePos(clickV);

        //        //turnReady.ShowCharacterActionPanel(SelChara.Cb.cid);
        //        SetSelClear();

        //        mouseInput.Mouse.MouseClick.performed += OnClick;
        //    }
        //}


        //public void OnClickSkillDirection(InputAction.CallbackContext context)
        //{

        //    if (SelAction == null || !(SelAction is SkillBase))
        //    {
        //        mouseInput.Mouse.MouseClick.performed += OnClick;
        //        mouseInput.Mouse.MouseClick.performed -= OnClickSkillDirection;

        //        SelChara = null;
        //        SelAction = null;

        //        return;
        //    }

        //    Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();

        //    // 클릭 된 좌표 맵 좌표로 변환
        //    float clickX = map.WorldToCell(Camera.main.ScreenToWorldPoint(mousePosition)).x;
        //    float charaX = SelChara.TempTilePos.x;


        //    // 클릭 된 좌표가 선택된 캐릭터의 오른쪽 있다면 왼쪽 하이라이트 및 방향 선택
        //    if (charaX < clickX)
        //    {
        //        //highLighter.HighlightMap(SelChara.TempTilePos, SelChara.TempTilePos.y%2==0 ? ((SkillBase)SelAction).areaEvenY : ((SkillBase)SelAction).areaOddY);

        //        if (SelOk > 0)
        //        {
        //            // 확정
        //            data.CharaActionData[SelChara.Cb.cid].AddSkillAction(ActionType.Skill, ((SkillBase)SelAction).sid, SkillDicection.Right);

        //            //turnReady.ShowCharacterActionPanel(SelChara.Cb.cid);
        //            SetSelClear();

        //            mouseInput.Mouse.MouseClick.performed += OnClick;
        //        }
        //        else
        //        {
        //            // 전에 다른 방향이었을 경우
        //            SelOk = 1;
        //        }
        //    }
        //    else
        //    {
        //        //highLighter.HighlightMapXReverse(SelChara.TempTilePos, SelChara.TempTilePos.y % 2 == 0 ? SelAction.areaEvenY : SelAction.areaOddY);

        //        if (SelOk < 0)
        //        {
        //            // 확정
        //            data.CharaActionData[SelChara.Cb.cid].AddSkillAction(ActionType.Skill, ((SkillBase)SelAction).sid, SkillDicection.Left);

        //            //turnReady.ShowCharacterActionPanel(SelChara.Cb.cid);
        //            SetSelClear();

        //            mouseInput.Mouse.MouseClick.performed += OnClick;
        //        }
        //        else
        //        {
        //            SelOk = -1;
        //        }
        //    }

        //    // 일단 스킬로 인한 자신의 위치 변경 내용은 없음
        //}

        public void SetSelSkill(SID sid)
        {
            SelAction = SkillManager.GetData(sid);
        }

        public void SetSelSkill(SkillBase sb)
        {
            SelAction = sb;

            //highLighter.HighlightMap(SelChara.TempTilePos, SelChara.TempTilePos.y % 2 == 0 ? sb.areaEvenY : sb.areaOddY);

        }

        public void StartControl()
        {
            mouseInput.Mouse.MouseClick.performed += OnClick;
        }

        public void SetSelClear()
        {
            SelChara = null;
            SelAction = null;
            SelOk = 0;

            mouseInput.Mouse.MouseClick.performed -= OnClick;
            //mouseInput.Mouse.MouseClick.performed -= OnClickSkillDirection;

            //showingSkillManager.ShowSkillPanel(-1);

            //highLighter.ClearHighlight();
            HighlightCharacterClear();
        }

        public void SetSelMove()
        {
            SelAction = MoveManager.MoveData;

            //Matrix4x4 groundTile = Matrix4x4.TRS(new Vector3(0, 0f, 0), Quaternion.Euler(0f, 0f, 0f), Vector3.one);
            //Matrix4x4 elevatedTile = Matrix4x4.TRS(new Vector3(0, 0.2f, 0), Quaternion.Euler(0f, 0f, 0f), Vector3.one/*scale 조정*/);
            //if (map.GetTile<CustomTile>(SelChara.TempTilePos).getCharCount() < 2)
            //{
            //    map.SetTransformMatrix(SelChara.TempTilePos, elevatedTile);
            //    highLighter.ChangeTileHeight(SelChara.TempTilePos, elevatedTile);
            //}
            //else
            //map.SetTransformMatrix(SelChara.TempTilePos, groundTile);

            /*TilemapControl fTiles = GameObject.Find("SecondTiles").GetComponent<TilemapControl>();

            map.SetTileFlags(SelChara.TempTilePos, TileFlags.None);
            map.SetColor(SelChara.TempTilePos, new Color(1, 1, 1, 0));

            Sprite sprite = map.GetTile<CustomTile>(SelChara.TempTilePos).sprite;
            fTiles.activateTile(map.CellToWorld(SelChara.TempTilePos), 2, sprite);*/

            //highLighter.HighlightMap(SelChara.TempTilePos, SelChara.TempTilePos.y % 2 == 0 ? SelAction.areaEvenY : SelAction.areaOddY);

            
        }

        public void SetSelChara(CID cid)
        {
            SelChara = null;

            Character c = data.GetCharacter(cid);


            if (c == null)
            {
                Debug.LogErrorFormat("Can not select - cid: {0}", cid);
                return;
            }

            SelAction = null;

            SelChara = c;
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
            foreach (CID c in data.CharacterObjects.Keys)
            {
                data.CharacterObjects[c].transform.localScale = new Vector3(0.7f, 0.7f, 1);
            }
        }

        public void HighlightCharacter(CID cid)
        {
            foreach(CID c in data.CharacterObjects.Keys)
            {
                if (c == cid)
                {
                    data.CharacterObjects[c].transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    data.CharacterObjects[c].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                }
                
            }
            
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
                        //Vector3 posAdaptation = map.GetCellCenterWorld(position); // 타일 한 칸에 한 캐릭터만 배치
                        //posAdaptation.y += (float)0.1; // 타일 한 칸의 중앙에 캐릭터 배치
                        deployPosition.Add(position); // Vector3Int.FloorToInt(posAdaptation)
                    }
                }
                else
                {
                    if (map.HasTile(position) && position.x > 0 && map.GetSprite(position).name == "tileWater_full")
                    {
                        //Vector3 posAdaptation = map.GetCellCenterWorld(position); // 타일 한 칸에 한 캐릭터만 배치
                        //posAdaptation.y += (float)0.1; // 타일 한 칸의 중앙에 캐릭터 배치
                        deployPosition.Add(position); // Vector3Int.FloorToInt(posAdaptation)
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
                //Destroy();
            }
            if (hit.collider == null) // Instantiate하면 Box Collider 2D 옵션이 꺼져 있음
            {
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector3Int clickV = map.WorldToCell(mousePosition);
                bool keyExists = characters.ContainsKey(clickV);
                if (PickManager.Instance.ClickedBtn != null && map.HasTile(clickV) && keyExists == false && deployCounter < 3)
                {
                    mousePosition = map.GetCellCenterWorld(clickV); // 타일 한 칸에 한 캐릭터만 배치
                    mousePosition.y += (float)0.1; // 타일 한 칸의 중앙에 캐릭터 배치
                    Debug.Log(mousePosition);
                    if (PhotonNetwork.IsMasterClient)
                    {
                        if (mousePosition.x < 0 && map.GetSprite(clickV).name == "tileWater_full")
                        {
                            newCharacters[deployCounter] = Instantiate(PickManager.Instance.ClickedBtn.CharacterPrefab, mousePosition, Quaternion.identity);
                            newCharacters[deployCounter].transform.parent = MasterClientCharacters.transform;
                            newCharacters[deployCounter].GetComponent<BoxCollider2D>().enabled = true;
                            PickManager.Instance.PickChance();
                            characters.Add(clickV, newCharacters[deployCounter]);
                            deployCounter++;
                        }
                    }
                    else
                    {
                        if (mousePosition.x > 0 && map.GetSprite(clickV).name == "tileWater_full")
                        {
                            newCharacters[deployCounter] = Instantiate(PickManager.Instance.ClickedBtn.CharacterPrefab, mousePosition, Quaternion.identity);
                            newCharacters[deployCounter].transform.parent = ClientCharacters.transform;
                            newCharacters[deployCounter].GetComponent<SpriteRenderer>().flipX = true;
                            newCharacters[deployCounter].GetComponent<BoxCollider2D>().enabled = true;
                            PickManager.Instance.PickChance(); // 버튼 Prefab 해제
                            characters.Add(clickV, newCharacters[deployCounter]);
                            deployCounter++;
                        }
                    }
                    //Debug.Log(pickManager.ClickedBtn.CharacterPrefab.name + " placed at " + mousePosition);
                    
                    
                }
            }
        }

        public void RandomDeployCharacter()
        {
            if (deployCounter < 3)
            {
                if (deployPosition != null)
                {
                    for (int i = 4; i >= deployCounter; i--)
                    {
                        var randomNum = Random.Range(0, deployPosition.Count);
                        //Debug.Log(randomNum);
                        randomNumList.Add(randomNum);
                        if (randomNumList.Contains(randomNum))
                        {
                            randomNum = Random.Range(0, deployPosition.Count);
                            //Debug.Log(randomNum);
                        }
                        var randomPosition = deployPosition[randomNum];
                        bool keyCheck = characters.ContainsKey(deployPosition[randomNum]);
                        if (keyCheck == false)
                        {
                            //Vector2 posAdaptation = map.GetCellCenterWorld(randomPosition); // 타일 한 칸에 한 캐릭터만 배치
                            //posAdaptation.y += (float)0.1; // 타일 한 칸의 중앙에 캐릭터 배치
                            //randomPosition.y -= (int)0.2;
                            var randomNum2 = Random.Range(0, character.Length);
                            newCharacters[deployCounter] = Instantiate(character[randomNum2], randomPosition, Quaternion.identity); // Vector3Int.FloorToInt(posAdaptation)
                            newCharacters[deployCounter].transform.parent = ClientCharacters.transform;
                            newCharacters[deployCounter].GetComponent<SpriteRenderer>().flipX = true;
                            newCharacters[deployCounter].GetComponent<BoxCollider2D>().enabled = true;
                            characters.Add(randomPosition, newCharacters[deployCounter]);
                            deployCounter++;
                        }
                    }
                }
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
                // selChara.SetTilePos((Vector2Int)gridPosition); 필요?
                Debug.Log("tile selected");
            }
            else if (!map.HasTile(gridPosition))
                Debug.Log("no tile selected");
            else
            {
                Debug.Log("더 이상 추가 불가");
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
                Debug.Log("더 이상 추가 불가");
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
