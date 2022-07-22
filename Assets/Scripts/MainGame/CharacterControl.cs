using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace KWY
{
    public class CharacterControl : MonoBehaviour
    {
        [SerializeField]
        MainGameData data;

        [SerializeField]
        Tilemap map;

        [SerializeField]
        ManageShowingSkills showingSkillManager;

        [SerializeField]
        MapHighLighter highLighter;

        [SerializeField]
        TurnReady turnReady;

        [SerializeField]
        RayTest ray;

        //[SerializeField]
        //skillSpawner skillSpawner;


        public Character SelChara { get; private set; }
        public ActionBase SelAction { get; private set; }


        #region Private Fields
        MouseInput mouseInput;

        private int SelOk = 0; // 음수: left, 양수: right -> |2| 가 되었을 때 액션 확정

        private skillSpawner skillSpawner;
        #endregion

        #region Public Methods

        public void OnChooseSkillDirectionMode()
        {
            // 캐릭터 선택 후 맵에 있는 캐릭터 클릭으로 캐릭터 선택할 수 있는 거 잠시 빼기
            mouseInput.Mouse.MouseClick.performed -= OnClick;

            // 방향 정할 수 있는 이벤트 넣기
            mouseInput.Mouse.MouseClick.performed += OnClickSkillDirection;
        }

        public void OnChooseMoveDirectionmode()
        {
            // 캐릭터 선택 후 맵에 있는 캐릭터 클릭으로 캐릭터 선택할 수 있는 거 잠시 빼기
            mouseInput.Mouse.MouseClick.performed -= OnClick;

            // 방향 정할 수 있는 이벤트 넣기
            mouseInput.Mouse.MouseClick.performed += OnClickMoveDirection;
        }

        public void OnClickMoveDirection(InputAction.CallbackContext context)
        {
            if (SelChara == null || !(SelAction is MoveBase)) return;

            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();

            // 바로 WorldToCell 함수에 집어넣지 말것! (???)
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // 사용 x
            // Vector3Int clickV = map.WorldToCell(mouseInput.Mouse.MouseClick.performed += OnClickMoveDirection;);

            // 클릭 된 좌표 맵 좌표로 변환
            Vector3Int clickV = map.WorldToCell(mousePosition);
            Vector3Int charaV = SelChara.TempTilePos;

            Vector2Int deltaXY = (Vector2Int)clickV - (Vector2Int)charaV;

            if (map.HasTile(clickV) && (SelChara.TempTilePos.y % 2 == 0 ? SelAction.areaEvenY : SelAction.areaOddY).Contains(deltaXY))
            {
                data.CharaActionData[SelChara.Cb.cid].AddMoveAction(ActionType.Move, (int)deltaXY.x, (int)deltaXY.y, SelChara.TempTilePos.y%2!=0);

                // 이동 넣었을 경우 하이라이트를 위한 임시 좌표 변경
                SelChara.SetTilePos(clickV);

                turnReady.ShowCharacterActionPanel(SelChara.Cb.cid);
                SetSelClear();

                mouseInput.Mouse.MouseClick.performed += OnClick;
            }
        }


        public void OnClickSkillDirection(InputAction.CallbackContext context)
        {

            if (SelAction == null || !(SelAction is SkillBase))
            {
                mouseInput.Mouse.MouseClick.performed += OnClick;
                mouseInput.Mouse.MouseClick.performed -= OnClickSkillDirection;

                SelChara = null;
                SelAction = null;

                return;
            }

            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();

            // 클릭 된 좌표 맵 좌표로 변환
            int clickX = map.WorldToCell(Camera.main.ScreenToWorldPoint(mousePosition)).x;
            int clickY = map.WorldToCell(Camera.main.ScreenToWorldPoint(mousePosition)).y;
            float charaX = SelChara.TempTilePos.x;
            skillSpawner = ((SkillBase)SelAction).area;

            // 클릭 된 좌표가 선택된 캐릭터의 오른쪽 있다면 왼쪽 하이라이트 및 방향 선택
            if (charaX < clickX)
            {
                highLighter.HighlightMap(SelChara.TempTilePos, SelChara.TempTilePos.y%2==0 ? ((SkillBase)SelAction).areaEvenY : ((SkillBase)SelAction).areaOddY);

                if (SelOk > 0)
                {
                    // 확정
                    data.CharaActionData[SelChara.Cb.cid].AddSkillAction(ActionType.Skill, ((SkillBase)SelAction).sid, SkillDicection.Right);

                    if(((SkillBase)SelAction).areaAttack)
                    {
                        Vector3Int v = new Vector3Int(clickX, clickY, 0);
                        skillSpawner.Activate(map.CellToWorld(v));
                        skillSpawner.Destroy(((SkillBase)SelAction).triggerTime);   // triggerTime만큼 스킬 지속후 삭제
                    }
                    else
                    {
                        ray.CurvedMultipleRay(map.CellToWorld(SelChara.TempTilePos), ((SkillBase)SelAction), ((SkillBase)SelAction).directions, true, ((SkillBase)SelAction).directions.Count);
                    }

                    turnReady.ShowCharacterActionPanel(SelChara.Cb.cid);
                    SetSelClear();

                    mouseInput.Mouse.MouseClick.performed += OnClick;
                }
                else
                {
                    // 전에 다른 방향이었을 경우
                    SelOk = 1;
                }
            }
            else
            {
                highLighter.HighlightMapXReverse(SelChara.TempTilePos, SelChara.TempTilePos.y % 2 == 0 ? SelAction.areaEvenY : SelAction.areaOddY);

                if (SelOk < 0)
                {
                    // 확정
                    data.CharaActionData[SelChara.Cb.cid].AddSkillAction(ActionType.Skill, ((SkillBase)SelAction).sid, SkillDicection.Left);

                    if (((SkillBase)SelAction).areaAttack)
                    {
                        Vector3Int v = new Vector3Int(clickX, clickY, 0);
                        skillSpawner.Activate(map.CellToWorld(v));
                        skillSpawner.Destroy(((SkillBase)SelAction).triggerTime);
                    }
                    else
                    {
                        ray.CurvedMultipleRay(map.CellToWorld(SelChara.TempTilePos), ((SkillBase)SelAction), ((SkillBase)SelAction).directions, false, ((SkillBase)SelAction).directions.Count);
                    }

                    turnReady.ShowCharacterActionPanel(SelChara.Cb.cid);
                    SetSelClear();

                    mouseInput.Mouse.MouseClick.performed += OnClick;
                }
                else
                {
                    SelOk = -1;
                }
            }

            // 일단 스킬로 인한 자신의 위치 변경 내용은 없음
        }

        public void SetSelSkill(SID sid)
        {
            SelAction = SkillManager.GetData(sid);
        }

        public void SetSelSkill(SkillBase sb)
        {
            SelAction = sb;

            highLighter.HighlightMap(SelChara.TempTilePos, SelChara.TempTilePos.y % 2 == 0 ? sb.areaEvenY : sb.areaOddY);
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
            mouseInput.Mouse.MouseClick.performed -= OnClickSkillDirection;

            showingSkillManager.ShowSkillPanel(-1);

            highLighter.ClearHighlight();
            HighlightCharacterClear();
        }

        public void SetSelMove()
        {
            SelAction = MoveManager.MoveData;

            highLighter.HighlightMap(SelChara.TempTilePos, SelChara.TempTilePos.y % 2 == 0 ? SelAction.areaEvenY : SelAction.areaOddY);
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
            showingSkillManager.ShowSkillPanel(data.GetCharacterNth(cid));
            HighlightCharacter(cid);
            highLighter.ClearHighlight();

            mouseInput.Mouse.MouseClick.performed += OnClick;
            mouseInput.Mouse.MouseClick.performed -= OnClickSkillDirection;
            mouseInput.Mouse.MouseClick.performed -= OnClickMoveDirection;
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


        public void OnClick(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                CID cid = hit.collider.gameObject.GetComponent<Character>().Cb.cid;
                SetSelChara(cid);
            }
        }

        #endregion


        #region Private Methods

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            // 반드시 여기서 할당해야됨! (선언에서 하면 error)
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
            
        }

        void Update()
        {
        }
        #endregion
    }
}
