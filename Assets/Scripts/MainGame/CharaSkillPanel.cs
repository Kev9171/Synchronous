using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KWY
{
    // attach this at CharaSkillPanel Prefab
    [RequireComponent(typeof(CanvasRenderer))]
    public class CharaSkillPanel : MonoBehaviour
    {
        public Image image;
        public TMP_Text costText;

        [Tooltip("Info 띄우는데 필요한 최소 클릭 시간; move 일 경우 없음")]
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
                    control.SetSelSkill(@base);
                    // 일단 스킬 클릭하면 맵에 해당 구역 하이라이트
                    //highLighter.HighlightMap(control.SelChara.transform.position, @base.area);

                    // 스킬 방향 선택 모드로
                    control.OnChooseSkillDirectionMode();
                }
                
            }
            else
            {
                if (control.SelAction == null)
                {
                    control.SetSelMove();
                    control.OnChooseMoveDirectionmode();
                }
                
                // 이동 방향 선택 할 수 있도록
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
                // move 는 정보 안보여줌
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

