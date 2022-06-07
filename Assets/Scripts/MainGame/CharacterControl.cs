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

        private int SelOk = 0; // ����: left, ���: right -> |2| �� �Ǿ��� �� �׼� Ȯ��
        #endregion

        #region Public Methods

        public void OnChooseSkillDirectionMode()
        {
            // ĳ���� ���� �� �ʿ� �ִ� ĳ���� Ŭ������ ĳ���� ������ �� �ִ� �� ��� ����
            mouseInput.Mouse.MouseClick.performed -= OnClick;

            // ���� ���� �� �ִ� �̺�Ʈ �ֱ�
            mouseInput.Mouse.MouseClick.performed += OnClickSkillDirection;
        }

        public void OnChooseMoveDirectionmode()
        {
            // ĳ���� ���� �� �ʿ� �ִ� ĳ���� Ŭ������ ĳ���� ������ �� �ִ� �� ��� ����
            mouseInput.Mouse.MouseClick.performed -= OnClick;

            // ���� ���� �� �ִ� �̺�Ʈ �ֱ�
            mouseInput.Mouse.MouseClick.performed += OnClickMoveDirection;
        }

        public void OnClickMoveDirection(InputAction.CallbackContext context)
        {
            if (SelChara == null || !(SelAction is MoveBase)) return;

            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();

            // �ٷ� WorldToCell �Լ��� ������� ����! (???)
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // ��� x
            // Vector3Int clickV = map.WorldToCell(mouseInput.Mouse.MouseClick.performed += OnClickMoveDirection;);

            // Ŭ�� �� ��ǥ �� ��ǥ�� ��ȯ
            Vector3Int clickV = map.WorldToCell(mousePosition);
            Vector3Int charaV = SelChara.TempTilePos;

            Vector2 deltaXY = (Vector2Int)clickV - (Vector2Int)charaV;

            Debug.LogFormat("mag: {0}", deltaXY.sqrMagnitude);

            if (map.HasTile(clickV) && deltaXY.sqrMagnitude <= 2)
            {
                data.CharaActionData[SelChara.Cb.cid].AddMoveAction(ActionType.Move, (int)deltaXY.x, (int)deltaXY.y);

                // �̵� �־��� ��� ���̶���Ʈ�� ���� �ӽ� ��ǥ ����
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

            // Ŭ�� �� ��ǥ �� ��ǥ�� ��ȯ
            float clickX = map.WorldToCell(Camera.main.ScreenToWorldPoint(mousePosition)).x;
            float charaX = SelChara.TempTilePos.x;


            // Ŭ�� �� ��ǥ�� ���õ� ĳ������ ������ �ִٸ� ���� ���̶���Ʈ �� ���� ����
            if (charaX < clickX)
            {
                highLighter.HighlightMap(SelChara.TempTilePos, ((SkillBase)SelAction).area);
                Debug.Log("Right");

                if (SelOk > 0)
                {
                    // Ȯ��
                    data.CharaActionData[SelChara.Cb.cid].AddSkillAction(ActionType.Skill, ((SkillBase)SelAction).sid, SkillDicection.Right);
                    Debug.Log("Add Right data");

                    turnReadyUI.UpdateCharaActions(SelChara.Cb.cid);
                    SetSelClear();

                    mouseInput.Mouse.MouseClick.performed += OnClick;
                }
                else
                {
                    // ���� �ٸ� �����̾��� ���
                    SelOk = 1;
                }
            }
            else
            {
                highLighter.HighlightMapXReverse(SelChara.TempTilePos, ((SkillBase)SelAction).area);
                Debug.Log("Left");

                if (SelOk < 0)
                {
                    // Ȯ��
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

            // �ϴ� ��ų�� ���� �ڽ��� ��ġ ���� ������ ����
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
