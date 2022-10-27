using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

using UI;

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
        MouseInput mouseInput;


        [Tooltip("Info ���µ� �ʿ��� �ּ� Ŭ�� �ð�; move �� ��� ����")]
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
            if (MainGameData.Instance.MyPlayer.Mp >= psb.cost)
            {
                mouseInput.Mouse.MouseClick.performed += OnClick;
                Debug.Log("��ų �ߵ�");
            }
            else
            {
                Debug.Log("���� ����");
                GameObject canvas = GameObject.Find("UICanvas");
                PanelBuilder.ShowFadeOutText(canvas.transform, "Not enough Mp to use this skill!");
            }
        }

        public void Skill1(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();

            if (MainGameData.Instance.MyPlayer.Skill1(SelChara, mousePosition))
            {
                mouseInput.Mouse.MouseClick.performed -= Skill1;
                MainGameData.Instance.MyPlayer.SubMp(psb.cost);
                Debug.Log($"���� �Ҹ�: {psb.cost}");
            }
            else
            {
                Debug.Log("??");
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                SelChara = hit.collider.gameObject.GetComponent<Character>();

                // �ڽ��� ĳ���͸� ���� �ǵ���
                if (!MainGameData.Instance.IsMyCharacter[SelChara.Pc.Id])
                {
                    Debug.Log("Selected Chara is not mine");
                    return;
                }

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