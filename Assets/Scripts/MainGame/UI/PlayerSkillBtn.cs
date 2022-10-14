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
            GameObject o = GameObject.Find("MainGameData");
            if (!o)
            {
<<<<<<< HEAD
                mouseInput.Mouse.MouseClick.performed += OnClick;
                GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
                gm.UpdatePlayerMP(-psb.cost);
=======
                Debug.LogError($"Can not find gameobject: 'MainGameData'");
                return;
            }

            MainGameData data = o.GetComponent<MainGameData>();
            if (!data)
            {
                Debug.LogError($"Can not find component: 'MainGameData' in gameobject named 'MainGameData'");
                return;
            }

            if (data.MyPlayer.Mp >= psb.cost)
            {
                data.MyPlayer.SubMp(psb.cost);
>>>>>>> kwy

                Debug.Log("스킬 발동");
            }
            else 
            {
                Debug.Log("마나 부족");
            }
        }

        public void Skill1(InputAction.CallbackContext context)
        {
            map = GameObject.Find("Tilemap").GetComponent<Tilemap>();
            chCtrl = GameObject.Find("CharacterControl").GetComponent<CharacterControl>();
            mouseInput.Mouse.MouseClick.performed += chCtrl.OnClick;

            if (chCtrl.SelChara == null) return;

            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();

            // 바로 WorldToCell 함수에 집어넣지 말것! (???)
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // 사용 x
            // Vector3Int clickV = map.WorldToCell(mouseInput.Mouse.MouseClick.performed += OnClickMoveDirection;);

            // 클릭 된 좌표 맵 좌표로 변환
            Vector3Int clickV = map.WorldToCell(mousePosition);

            if (map.HasTile(clickV))
            {
                SelChara.TilePos = clickV;
                Vector3 newPos = map.CellToWorld(clickV);
                newPos.y += 0.1f;
                SelChara.transform.position = newPos;
                mouseInput.Mouse.MouseClick.performed -= Skill1;
            }
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
