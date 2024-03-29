using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

using KWY;

namespace UI
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
        TMP_Text hpMaxLabel;
        [SerializeField]
        TMP_Text hpLabel;

        [SerializeField]
        Slider mpBar;
        [SerializeField]
        TMP_Text mpMaxLabel;
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

        [SerializeField]
        Canvas CanvasUI;

        [Tooltip("0 ~ 2 (위에서 부터)")]
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
        /// 처음 데이터를 넣는 함수; 한번만 호출 할 수 있도록
        /// </summary>
        /// <param name="cb">Chacter Base Data</param>
        public void Init(Character chararacter)
        {
            chara = chararacter;

            nameLabel.text = chara.Cb.characterName;

            charaImg.sprite = chara.Cb.icon;
            charaImg.preserveAspect = true;

            LoadBuffs();

            // set initial hp and mp
            hpMaxLabel.text = chara.MaxHp.ToString();
            mpMaxLabel.text = chara.MaxMp.ToString();

            hpLabel.text = chara.Hp.ToString();
            mpLabel.text = chara.Mp.ToString();

            hpBar.maxValue = chara.MaxHp;
            mpBar.maxValue = chara.MaxMp;
        }

        public void UpdateUI(Character c)
        {
            if (c.BreakDown)
            {
                charaImg.color = breakDownColor;
                ClearBuffs();
            }

            UpdateHP(c.Hp);
            UpdateMP(c.Mp);
        }

        /// <summary>
        /// Update Hp
        /// </summary>
        /// <param name="hp">now(updated) hp</param>
        public void UpdateHP(int hp)
        {
            int now = (int)hpBar.value;
            StartCoroutine(IEUpdateHp(hp - now));
        }

        public void UpdateMP(int mp)
        {
            int now = (int)mpBar.value;
            StartCoroutine(IEUpdateMp(mp - now));
        }

        public void UpdateBuffs()
        {
            LoadBuffs();
        }

        /// <summary>
        /// CharaInfoBtn 에 대한 OnClick CallBack 함수
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
                PanelBuilder.ShowFadeOutText(CanvasUI.transform, "This character is break down. You can not choose");
                return;
            }

            // 캐릭터 선택
            charaControl.SetSelChara(chara);
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

        private void LoadBuffs()
        {
            ClearBuffs();

            foreach (Buff bf in chara.Buffs)
            {
                AddBuff(bf.bb, bf.turn);
            }
        }

        private void AddBuff(BuffBase bb, int nTurn)
        {
            GameObject bPanel = Instantiate(buffPanelPrefab, buffPanel.transform);
            bPanel.GetComponent<BuffPanel>().SetData(bb, nTurn);
            buffPanelLists.Add(bPanel);
        }

        #endregion

        #region IEnumerator

        IEnumerator IEUpdateHp(int dv)
        {
            // 10 tick

            // tick 당 업데이트할 값
            float v = dv / 10f;
            for (float ft = 1f; ft >= 0; ft -= 0.1f)
            {
                hpBar.value += v;
                hpLabel.text = ((int)(hpBar.value)).ToString();
                yield return new WaitForSeconds(0.1f);
            }
            hpBar.value = chara.Hp;
            hpLabel.text = chara.Hp.ToString();
        }

        IEnumerator IEUpdateMp(int dv)
        {
            // TODO
            float v = dv / 10f;
            for (float ft = 1f; ft >= 0; ft -= 0.1f)
            {
                mpBar.value += v;
                mpLabel.text = ((int)(mpBar.value)).ToString();
                yield return new WaitForSeconds(0.1f);
            }
            mpBar.value = chara.Mp;
            mpLabel.text = chara.Mp.ToString();
        }

        #endregion
    }
}