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

        private int SelOk = 0; // ����: left, ���: right -> |2| �� �Ǿ��� �� �׼� Ȯ��

        private skillSpawner skillSpawner;
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

            Vector2Int deltaXY = (Vector2Int)clickV - (Vector2Int)charaV;

            if (map.HasTile(clickV) && (SelChara.TempTilePos.y % 2 == 0 ? SelAction.areaEvenY : SelAction.areaOddY).Contains(deltaXY))
            {
                data.CharaActionData[SelChara.Cb.cid].AddMoveAction(ActionType.Move, (int)deltaXY.x, (int)deltaXY.y, SelChara.TempTilePos.y%2!=0);

                // �̵� �־��� ��� ���̶���Ʈ�� ���� �ӽ� ��ǥ ����
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

            // Ŭ�� �� ��ǥ �� ��ǥ�� ��ȯ
            int clickX = map.WorldToCell(Camera.main.ScreenToWorldPoint(mousePosition)).x;
            int clickY = map.WorldToCell(Camera.main.ScreenToWorldPoint(mousePosition)).y;
            float charaX = SelChara.TempTilePos.x;
            skillSpawner = ((SkillBase)SelAction).area;

            // Ŭ�� �� ��ǥ�� ���õ� ĳ������ ������ �ִٸ� ���� ���̶���Ʈ �� ���� ����
            if (charaX < clickX)
            {
                highLighter.HighlightMap(SelChara.TempTilePos, SelChara.TempTilePos.y%2==0 ? ((SkillBase)SelAction).areaEvenY : ((SkillBase)SelAction).areaOddY);

                if (SelOk > 0)
                {
                    // Ȯ��
                    data.CharaActionData[SelChara.Cb.cid].AddSkillAction(ActionType.Skill, ((SkillBase)SelAction).sid, SkillDicection.Right);

                    if(((SkillBase)SelAction).areaAttack)
                    {
                        Vector3Int v = new Vector3Int(clickX, clickY, 0);
                        skillSpawner.Activate(map.CellToWorld(v));
                        skillSpawner.Destroy(((SkillBase)SelAction).triggerTime);   // triggerTime��ŭ ��ų ������ ����
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
                    // ���� �ٸ� �����̾��� ���
                    SelOk = 1;
                }
            }
            else
            {
                highLighter.HighlightMapXReverse(SelChara.TempTilePos, SelChara.TempTilePos.y % 2 == 0 ? SelAction.areaEvenY : SelAction.areaOddY);

                if (SelOk < 0)
                {
                    // Ȯ��
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

            // �ϴ� ��ų�� ���� �ڽ��� ��ġ ���� ������ ����
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
            // �ݵ�� ���⼭ �Ҵ��ؾߵ�! (���𿡼� �ϸ� error)
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
