using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class PlayerSkillBtn : MonoBehaviour
    {
        [SerializeField]
        TMP_Text costLabel;

        [SerializeField]
        Image icon;

        PlayerSkillBase psb;

        Character SelChara;
        Tilemap map;
        MouseInput mouseInput;
        CharacterControl chCtrl;

        MainGameData data;

        Simulation simulation;

        [Tooltip("Info 띄우는데 필요한 최소 클릭 시간; move 일 경우 없음")]
        public float minClickTime = 1;


        #region Private Fields

        private float clickTime;
        private bool isClick;

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

        public void SetData(PlayerSkillBase psb)
        {
            costLabel.text = psb.cost.ToString();
            icon.sprite = psb.icon;

            this.psb = psb;
        }

        public void OnClickUseSkill()
        {
            MainGameData data = GameObject.Find("GameData").GetComponent<MainGameData>();

            if (data.MyPlayer.Mp >= psb.cost)
            {
                mouseInput.Mouse.MouseClick.performed += OnClick;
                //GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
                data.MyPlayer.SubMp(psb.cost);

                Debug.Log("스킬 발동");
            }
            else
            {
                Debug.Log("마나 부족");
            }
        }

        public void Skill1(InputAction.CallbackContext context)
        {
            if (data.MyPlayer.Skill1(SelChara))
            {
                mouseInput.Mouse.MouseClick.performed -= Skill1;
            }

            /*map = GameObject.Find("Tilemap").GetComponent<Tilemap>();
            chCtrl = GameObject.Find("CharacterControl").GetComponent<CharacterControl>();
            //mouseInput.Mouse.MouseClick.performed += chCtrl.OnClick;

            if (SelChara == null) return;
            Debug.Log("clicked " + SelChara);
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();

            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector3Int clickV = map.WorldToCell(mousePosition);

            if (map.HasTile(clickV))
            {
                TilemapControl TCtrl = GameObject.Find("TilemapControl").GetComponent<TilemapControl>();
                if (clickV.y % 2 != SelChara.TilePos.y % 2)
                {
                    simulation.ChangeAction((int)SelChara.Cb.cid, clickV.y % 2, MoveManager.MoveData);
                }
                SelChara.Teleport(clickV);


                mouseInput.Mouse.MouseClick.performed -= Skill1;
            }*/
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                //CID cid = hit.collider.gameObject.GetComponent<Character>().Cb.cid;
                //chCtrl.SetSelChara(cid);
                SelChara = hit.collider.gameObject.GetComponent<Character>();
                mouseInput.Mouse.MouseClick.performed -= OnClick;
                mouseInput.Mouse.MouseClick.performed += Skill1;
            }
            else
                Debug.Log("no char");
        }


        public void ButtonUp()
        {
            isClick = false;

            if (clickTime >= minClickTime)
            {
                GameObject canvas = GameObject.Find("UICanvas");
                PanelBuilder.ShowPlayerSkillInfoPanel(canvas.transform, psb);
            }
            else
            {
                OnClickUseSkill();
            }
        }

        public void ButtonDown()
        {
            isClick = true;
        }

        #region MonoBehaviour CallBacks

        private void Start()
        {
            if (!data)
            {
                GameObject o = GameObject.Find("GameData");

                if (!o)
                {
                    Debug.Log("Can not find game object named: GameData");
                }

                data = o.GetComponent<MainGameData>();

                if (!data)
                {
                    Debug.Log("Can not find component at GameData: MainGameData");
                }
            }

            GameObject oo = GameObject.Find("UICanvas");

            if (!oo)
            {
                Debug.Log("Can not find game object named: UICanvas");
            }

            simulation = oo.GetComponent<Simulation>();
            if (!simulation)
            {
                Debug.Log("Can not find component at UICanvas: Simulation");
            }
        }
        private void Update()
        {
            if (isClick)
                clickTime += Time.deltaTime;
            else
                clickTime = 0;
        }
        #endregion

    }
}