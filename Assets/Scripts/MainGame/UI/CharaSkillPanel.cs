using UnityEngine;
using UnityEngine.UI;
using TMPro;

using KWY;

namespace UI
{
    // attach this at CharaSkillPanel Prefab
    [RequireComponent(typeof(CanvasRenderer))]
    public class CharaSkillPanel : MonoBehaviour
    {
        public Image image;
        public TMP_Text costText;

        [Tooltip("Info ���µ� �ʿ��� �ּ� Ŭ�� �ð�; move �� ��� ����")]
        public float minClickTime = 1;

        #region Private Fields

        private float clickTime;
        private bool isClick;

        private ActionBase ab;
        
        #endregion

        public void SetData(ActionBase ab)
        {
            image.sprite = ab.icon;
            costText.text = ab.cost.ToString();
            this.ab = ab;
        }

        public void OnClickSetOrder()
        {
            MapHighLighter highLighter = GameObject.FindGameObjectWithTag("MapHighlighter").GetComponent<MapHighLighter>();
            CharacterControl control = GameObject.FindGameObjectWithTag("CharacterControl").GetComponent<CharacterControl>();

            
            if (ab is SkillBase @base)
            {
                if (control.SelAction == null)
                {
                    if (control.SetSelSkill(@base))
                    {
                        // ��ų ���� ���� ����
                        control.OnChooseSkillDirectionMode();
                    }
                }
                
            }
            else
            {
                if (control.SelAction == null)
                {
                    control.SetSelMove();
                    control.OnChooseMoveDirectionmode();
                }
            }
            
        }

        public void ButtonDown()
        {
            isClick = true;
        }
        public void ButtonUp()
        {
            isClick = false;

            if (clickTime >= minClickTime)
            {
                // move �� ���� �Ⱥ�����
                if (ab is SkillBase @base)
                {
                    GameObject canvas = GameObject.Find("UICanvas");

                    PanelBuilder.ShowSkillInfoPanel(canvas.transform, @base);
                }
            }
            else
            {
                OnClickSetOrder();
            }
        }

        private void Update()
        {
            if (isClick)
                clickTime += Time.deltaTime;
            else
                clickTime = 0;
        }
    }
}

