using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

using System.Collections.Generic;

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
        UIControlReady turnReadyUI;


        public Character SelChara { get; private set; }
        public ActionBase SelAction { get; private set; }


        #region Private Fields
        MouseInput mouseInput;

        private int SelOk = 0; // 음수: left, 양수: right -> |2| 가 되었을 때 액션 확정
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

            Vector2 deltaXY = (Vector2Int)clickV - (Vector2Int)charaV;

            Debug.LogFormat("mag: {0}", deltaXY.sqrMagnitude);

            if (map.HasTile(clickV) && deltaXY.sqrMagnitude <= 2)
            {
                data.CharaActionData[SelChara.Cb.cid].AddMoveAction(ActionType.Move, (int)deltaXY.x, (int)deltaXY.y);

                // 이동 넣었을 경우 하이라이트를 위한 임시 좌표 변경
                SelChara.SetTilePos(clickV);

                turnReadyUI.UpdateCharaActions(SelChara.Cb.cid);
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
            float clickX = map.WorldToCell(Camera.main.ScreenToWorldPoint(mousePosition)).x;
            float charaX = SelChara.TempTilePos.x;


            // 클릭 된 좌표가 선택된 캐릭터의 오른쪽 있다면 왼쪽 하이라이트 및 방향 선택
            if (charaX < clickX)
            {
                highLighter.HighlightMap(SelChara.TempTilePos, ((SkillBase)SelAction).area);
                Debug.Log("Right");

                if (SelOk > 0)
                {
                    // 확정
                    data.CharaActionData[SelChara.Cb.cid].AddSkillAction(ActionType.Skill, ((SkillBase)SelAction).sid, SkillDicection.Right);
                    Debug.Log("Add Right data");

                    turnReadyUI.UpdateCharaActions(SelChara.Cb.cid);
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
                highLighter.HighlightMapXReverse(SelChara.TempTilePos, ((SkillBase)SelAction).area);
                Debug.Log("Left");

                if (SelOk < 0)
                {
                    // 확정
                    data.CharaActionData[SelChara.Cb.cid].AddSkillAction(ActionType.Skill, ((SkillBase)SelAction).sid, SkillDicection.Left);
                    Debug.Log("Add Left data");

                    turnReadyUI.UpdateCharaActions(SelChara.Cb.cid);
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

            highLighter.HighlightMap(SelChara.TempTilePos, sb.area);

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

            highLighter.ClearHighlight();
            HighlightCharacterClear();
        }

        public void SetSelMove()
        {
            SelAction = MoveManager.MoveData;

            string t = "";
            foreach(var v in MoveManager.MoveData.area)
            {
                t += string.Format("{0}, ", v);
            }
            Debug.Log(t);


            highLighter.HighlightMap(SelChara.TempTilePos, SelAction.area);
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

            Debug.LogFormat("tPos0: {0}", SelChara.TempTilePos.GetHashCode());

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
