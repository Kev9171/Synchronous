using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

using UI;

namespace KWY
{
    public class CharacterControl : MonoBehaviour
    {
        [SerializeField]
        MainGameData data;

        [SerializeField]
        Tilemap map;

        [SerializeField]
        MapHighLighter highLighter;

        [SerializeField]
        TurnReady turnReady;

        [SerializeField]
        RayTest ray;

        [SerializeField]
        CharacterUIHandler characterUIHandler;

        [SerializeField]
        Canvas canvas;

        public Character SelChara { get; private set; }
        public ActionBase SelAction { get; private set; }


        #region Private Fields
        MouseInput mouseInput;

        private int SelOk = 0; // 음수: left, 양수: right -> |2| 가 되었을 때 액션 확정
        private SkillSpawner skillSpawner;
        private int tempClickX, tempClickY = -int.MaxValue;
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

            // 클릭 된 좌표 맵 좌표로 변환
            Vector3Int clickV = map.WorldToCell(mousePosition);
            Vector3Int charaV = SelChara.TempTilePos;

            Vector2Int deltaXY = (Vector2Int)clickV - (Vector2Int)charaV;

            if (map.HasTile(clickV) && (SelChara.TempTilePos.y % 2 == 0 ? SelAction.areaEvenY : SelAction.areaOddY).Contains(deltaXY))
            {
                data.CharaActionData[SelChara.Pc.Id].AddMoveAction(ActionType.Move, (int)deltaXY.x, (int)deltaXY.y, SelChara.TempTilePos.y%2!=0);

                // 이동 넣었을 경우 하이라이트를 위한 임시 좌표 변경
                SelChara.SetTilePos(clickV);

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
                //highLighter.HighlightMap(SelChara.TempTilePos, SelChara.TempTilePos.y % 2 == 0 ? ((SkillBase)SelAction).areaEvenY : ((SkillBase)SelAction).areaOddY);
                if (((SkillBase)SelAction).areaAttack)
                {
                    highLighter.HighlightMap(map.CellToWorld(new Vector3Int(clickX, clickY, 0)), ((SkillBase)SelAction));
                }
                else
                {
                    highLighter.HighlightMap(map.CellToWorld(SelChara.TempTilePos), ((SkillBase)SelAction), true);
                }

                if (tempClickX == clickX && tempClickY == clickY)
                {
                    // 같은 위치를 클릭했을 경우
                    SelOk = 1;
                }
                else
                {
                    SelOk = 0;
                }

                if (SelOk > 0)
                {
                    // 확정
                    data.CharaActionData[SelChara.Pc.Id].AddSkillAction(ActionType.Skill, ((SkillBase)SelAction).sid, SkillDicection.Right, clickX, clickY);
                    SelChara.TempMp -= SelAction.cost;
                    /*if (((SkillBase)SelAction).areaAttack)
                    {
                        Vector3Int v = new Vector3Int(clickX, clickY, 0);
                        skillSpawner.Activate(map.CellToWorld(v));
                        skillSpawner.Destroy(((SkillBase)SelAction).triggerTime);   // triggerTime만큼 스킬 지속후 삭제
                    }
                    else
                    {
                        ray.CurvedMultipleRay(map.CellToWorld(SelChara.TempTilePos), ((SkillBase)SelAction), ((SkillBase)SelAction).directions, true, ((SkillBase)SelAction).directions.Count);
                    }*/

                    SetSelClear();

                    mouseInput.Mouse.MouseClick.performed += OnClick;
                }
            }
            else
            {
                //highLighter.HighlightMapXReverse(SelChara.TempTilePos, SelChara.TempTilePos.y % 2 == 0 ? SelAction.areaEvenY : SelAction.areaOddY);
                if (((SkillBase)SelAction).areaAttack)
                {
                    highLighter.HighlightMap(map.CellToWorld(new Vector3Int(clickX, clickY, 0)), ((SkillBase)SelAction));
                }
                else
                {
                    highLighter.HighlightMap(map.CellToWorld(SelChara.TempTilePos), ((SkillBase)SelAction), false);
                }

                if (tempClickX == clickX && tempClickY == clickY)
                {
                    SelOk = -1;
                }
                else
                {
                    SelOk = 0;
                }

                if (SelOk < 0)
                {
                    // 확정
                    data.CharaActionData[SelChara.Pc.Id].AddSkillAction(ActionType.Skill, ((SkillBase)SelAction).sid, SkillDicection.Left, clickX, clickY);
                    SelChara.TempMp -= SelAction.cost;
                    /*if (((SkillBase)SelAction).areaAttack)
                    {
                        Vector3Int v = new Vector3Int(clickX, clickY, 0);
                        skillSpawner.Activate(map.CellToWorld(v));
                        skillSpawner.Destroy(((SkillBase)SelAction).triggerTime);
                    }
                    else
                    {
                        ray.CurvedMultipleRay(map.CellToWorld(SelChara.TempTilePos), ((SkillBase)SelAction), ((SkillBase)SelAction).directions, false, ((SkillBase)SelAction).directions.Count);
                    }*/

                    SetSelClear();

                    mouseInput.Mouse.MouseClick.performed += OnClick;
                }
            }

            tempClickX = clickX;
            tempClickY = clickY;
        }

        public void SetSelSkill(SID sid)
        {
            SelAction = SkillManager.GetData(sid);
        }

        public bool SetSelSkill(SkillBase sb)
        {
            SelAction = sb;

            if (SelAction.cost > SelChara.TempMp)
            {
                SelAction = null;
                PanelBuilder.ShowFadeOutText(canvas.transform, "Not enough Mp to use this skill!");
                return false;
            }

            if (((SkillBase)SelAction).areaAttack)
            {
                highLighter.HighlightMap(map.CellToWorld(SelChara.TempTilePos), ((SkillBase)SelAction));
            }
            else
            {
                highLighter.HighlightMap(map.CellToWorld(SelChara.TempTilePos), ((SkillBase)SelAction), true);
            }

            return true;
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

            characterUIHandler.HideAllSkillSelPanel();

            highLighter.ClearHighlight();
            HighlightCharacterClear();
        }

        public void SetSelMove()
        {
            SelAction = MoveManager.MoveData;

            highLighter.HighlightMap(SelChara.TempTilePos, SelChara.TempTilePos.y % 2 == 0 ? SelAction.areaEvenY : SelAction.areaOddY);
        }

        public void SetSelChara(Character chara)
        {
            SelChara = chara;
            SelAction = null;

            // 스킬 선택 패널
            characterUIHandler.ShowSkillSelPanel(SelChara);
            HighlightCharacter(SelChara);


            highLighter.ClearHighlight();

            mouseInput.Mouse.MouseClick.performed += OnClick;
            mouseInput.Mouse.MouseClick.performed -= OnClickSkillDirection;
            mouseInput.Mouse.MouseClick.performed -= OnClickMoveDirection;

            tempClickX = int.MaxValue;
            tempClickY = int.MaxValue;
        }

        public void HighlightCharacterClear()
        {
            foreach(PlayableCharacter p in data.PCharacters.Values)
            {
                p.CharaObject.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            }
        }

        public void HighlightCharacter(Character chara)
        {
            foreach (PlayableCharacter c in data.MyTeamCharacter)
            {
                if (c.Chara.Equals(chara))
                {
                    c.Chara.gameObject.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    c.Chara.gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                }
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null && hit.collider.gameObject.TryGetComponent<Character>(out var ch))
            {
                //Character c = hit.collider.gameObject.GetComponent<Character>();
                SetSelChara(ch);
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
        }

        void Update()
        {
            if (mouseInput.Mouse.MouseClick.IsPressed())
            {
                //mouseInput.Mouse.MouseClick.performed -= CharMoveSelect;
                //mouseInput.Mouse.MouseClick.performed -= CharAttackSelect;
                //mouseInput.Mouse.MouseClick.performed += OnClick;
            }
        }

        #endregion
    }
}
