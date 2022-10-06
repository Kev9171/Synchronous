using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

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
        Slider hpBar;
        [SerializeField]
        TMP_Text hpLabel;

        [SerializeField]
        Slider mpBar;
        [SerializeField]
        TMP_Text mpLabel;

        [SerializeField]
        GameObject buffPanel;

        [SerializeField]
        Button charaInfoBtn;

        [SerializeField]
        private Image[] selectedActionImages;

        [SerializeField]
        CharacterControl charaControl;

        [Tooltip("0 ~ 2 (������ ����)")]
        [SerializeField]
        int nthCharacter;

        #region Private Fields

        private List<GameObject> buffPanelLists = new List<GameObject>();
        private Character chara;

        public bool Selectable = false;

        [SerializeField]
        private Color breakDownColor = new Color(0.66f, 0, 0, 0.7f);

        #endregion

        #region Public Methods

        /// <summary>
        /// ó�� �����͸� �ִ� �Լ�; �ѹ��� ȣ�� �� �� �ֵ���
        /// </summary>
        /// <param name="cb">Chacter Base Data</param>
        public void Init(Character chararacter, List<Buff> buffList)
        {
            chara = chararacter;

            nameLabel.text = chara.Cb.characterName;

            charaImg.sprite = chara.Cb.icon;

            LoadBuffs(buffList);

            UpdateHP(chara.Cb.hp);
            UpdateMP(0);
        }

        public void UpdateUI(Character c)
        {
            if (c.BreakDown)
            {
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
            float now = chara.Hp;
            float v = hpBar.value * chara.MaxHp;
            StartCoroutine(IEUpdateHp(now - v));
            //hpLabel.text = hp.ToString() + "/" + chara.MaxHp.ToString();
            //hpBar.value = hp / chara.MaxHp;
        }

        public void UpdateMP(float mp)
        {
            mpLabel.text = mp.ToString() + "/" + chara.MaxMp.ToString();
            mpBar.value = mp / chara.MaxMp;
        }

        public void AddBuff(BuffBase bb, int nTurn)
        {
            GameObject bPanel = Instantiate(buffPanelPrefab, buffPanel.transform);
            bPanel.GetComponent<BuffPanel>().SetData(bb, nTurn);
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
            GameObject canvas = GameObject.Find("UICanvas");
            PanelBuilder.ShowCharacterInfoPanel(canvas.transform, chara.Cb);
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
            if (!Selectable)
            {
                Debug.Log("It is not selectable now");
                return;
            }

            if (chara.BreakDown)
            {
                Debug.Log("This character is break down. You can not choose");
                return;
            }

            // ĳ���� ����
            charaControl.SetSelChara(chara);
        }

        /// <summary>
        /// SetActive(false)�� �Ǹ� find �� �� �����Ƿ� SetActive(false)�� �Ǳ� ���� ȣ���Ͽ� ���� �ֱ�
        /// </summary>
        /// <param name="characterInfoPanel">CharacterInfoPanel</param>
        /// <param name="buffInfoPanel">BuffInfoPanel</param>
        public void LoadInfoPanel(GameObject characterInfoPanel, GameObject buffInfoPanel)
        {
            Debug.Log("LoadInfoPanel");
            //this.characterInfoPanel = characterInfoPanel;
            //this.buffInfoPanel = buffInfoPanel;
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

        #region IEnumerator

        IEnumerator IEUpdateHp(float dv)
        {
            // 10 tick (2000ms���� ������Ʈ)

            // tick �� ������Ʈ�� ��
            float v = (dv / chara.MaxHp) / 10f;
            for (float ft = 1f; ft >= 0; ft -= 0.1f)
            {
                hpBar.value += v;
                hpLabel.text = Mathf.Floor(hpBar.value * chara.MaxHp).ToString();
                yield return new WaitForSeconds(0.1f);
            }
            
        }

        #endregion
    }
}
