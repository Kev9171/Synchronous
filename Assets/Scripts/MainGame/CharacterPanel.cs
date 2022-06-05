using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class CharacterPanel : MonoBehaviour
    {
        #region Prefabs

        [SerializeField]
        GameObject buffPanelPrefab;

        #endregion

        [SerializeField]
        TMP_Text nameLabel;

        [SerializeField]
        Image charaImg;

        [SerializeField]
        GameObject hpBar;
        [SerializeField]
        TMP_Text hpLabel;

        [SerializeField]
        GameObject mpBar;
        [SerializeField]
        TMP_Text mpLabel;

        [SerializeField]
        GameObject buffPanel;

        [SerializeField]
        Button charaInfoBtn;

        [SerializeField]
        GameObject showingSkillManager;

        [Tooltip("0 ~ 2 (������ ����)")]
        [SerializeField]
        int nthCharacter;

        private GameObject buffInfoPanel;

        private Image[] selectedActionImages;

        private GameObject characterInfoPanel;

        #region Private Fields

        private CharacterBase cb;
        private List<GameObject> buffPanelLists = new List<GameObject>();
        private bool selectable = true;

        [SerializeField]
        private Color breakDownColor = new Color(0.66f, 0, 0, 0.7f);

        #endregion

        #region Public Methods

        /// <summary>
        /// ó�� �����͸� �ִ� �Լ�; �ѹ��� ȣ�� �� �� �ֵ���
        /// </summary>
        /// <param name="cb">Chacter Base Data</param>
        public void SetData(CharacterBase cb, List<Buff> buffList)
        {
            nameLabel.text = cb.characterName;
            UpdateHP(cb.hp);
            UpdateMP(0);

            // Set data to infopanel
            // need

            charaImg.sprite = cb.icon;

            LoadBuffs(buffList);

            this.cb = cb;
        }

        public void SetPanelRef(GameObject charaInfo, GameObject buffInfo)
        {
            buffInfoPanel = buffInfo;
            characterInfoPanel = charaInfo;
            buffInfoPanel.SetActive(false);
            characterInfoPanel.SetActive(false);
        }

        public void UpdateData(Character c)
        {
            if (c.BreakDown)
            {
                selectable = false;
                charaImg.color = breakDownColor;
                ClearBuffs();
            }

            else
            {
                UpdateHP(c.Hp);
                UpdateMP(c.Mp);

                LoadBuffs(c.Buffs);
            }
        }


        public void UpdateHP(float hp)
        {
            // bar ���� ó�� �ʿ�
            hpLabel.text = hp.ToString();
        }

        public void UpdateMP(float mp)
        {
            // bar ���� ó�� �ʿ�
            mpLabel.text = mp.ToString();
        }


        public void AddBuff(BuffBase bb, int nTurn)
        {
            GameObject bPanel = Instantiate(buffPanelPrefab, buffPanel.transform);
            bPanel.GetComponent<BuffPanel>().SetData(bb, nTurn, buffInfoPanel);
            buffPanelLists.Add(bPanel);
        }

        public void ReduceTurn(int nTurn)
        {
            foreach(GameObject buff in buffPanelLists)
            {
                // test �ʿ� loop �߿� ��� �����ϰ� �־ ��� �Ǵ��� ��
                if (!buff.GetComponent<BuffPanel>().ReduceBuffTurnText(nTurn))
                {
                    buffPanelLists.Remove(buff);
                    Destroy(buff);
                }
            }
        }

        /// <summary>
        /// CharaInfoBtn �� ���� OnClick CallBack �Լ�
        /// </summary>
        public void CharaInfoBtnOnClick()
        {
            characterInfoPanel.GetComponent<CharacterInfoPanel>().SetData(cb);
            characterInfoPanel.SetActive(true);
        }

        public void SetSelActionImg(int nth, Sprite icon)
        {
            if (nth >=0 && nth <3)
                selectedActionImages[nth].sprite = icon;
            else
            {
                selectedActionImages[0].sprite = null;
                selectedActionImages[1].sprite = null;
                selectedActionImages[2].sprite = null;
            }
        }

        public void ResetSelActionImg()
        {
            selectedActionImages[0].sprite = null;
            selectedActionImages[1].sprite = null;
            selectedActionImages[2].sprite = null;
        }

        public void OnClickShowSkillPanel()
        {
            if (!selectable)
            {
                Debug.Log("This character is break down. You can not choose");
                return;
            }
            showingSkillManager.GetComponent<ManageShowingSkills>().ShowSkillPanel(nthCharacter);
        }

        #endregion

        #region Private Methods

        private void ClearBuffs()
        {
            foreach (GameObject buffPanel in buffPanelLists)
            {
                Destroy(buffPanel);
            }

            buffPanelLists.Clear();
        }

        private void LoadBuffs(List<Buff> buffs)
        {
            ClearBuffs();

            foreach (Buff bf in buffs)
            {
                //ReduceTurn(10); // temp
                AddBuff(bf.bb, bf.turn);
            }
        }



        #endregion


        #region MonoBehaviour CallBacks

        private void Start()
        {
            
        }

        #endregion

        
    }
}
