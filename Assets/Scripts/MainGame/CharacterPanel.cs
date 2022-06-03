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
        GameObject buffImgPrefab;

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
        GameObject buffInfoPanel;

        [SerializeField]
        Image[] selectedActionImages;

        [SerializeField]
        GameObject characterInfoPanel;

        #region Private Fields

        private CharacterBase cb;
        private List<GameObject> buffList = new List<GameObject>();

        #endregion

        #region Public Methods

        /// <summary>
        /// ó�� �����͸� �ִ� �Լ�; �ѹ��� ȣ�� �� �� �ֵ���
        /// </summary>
        /// <param name="cb">Chacter Base Data</param>
        public void SetData(CharacterBase cb)
        {
            nameLabel.text = cb.characterName;
            UpdateHP(cb.hp);
            UpdateMP(0);

            // Set data to infopanel
            // need

            charaImg.sprite = cb.icon;

            this.cb = cb;
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

        public void AddBuffImg(BuffBase bb, int nTurn)
        {
            GameObject bPanel = Instantiate(buffImgPrefab, buffPanel.transform);
            bPanel.GetComponent<BuffPanel>().SetData(bb, nTurn, buffInfoPanel);
            buffList.Add(bPanel);
        }

        public void ReduceTurn(int nTurn)
        {
            foreach(GameObject buff in buffList)
            {
                // test �ʿ� loop �߿� ��� �����ϰ� �־ ��� �Ǵ��� ��
                if (!buff.GetComponent<BuffPanel>().ReduceBuffTurnText(nTurn))
                {
                    buffList.Remove(buff);
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

        #endregion

        #region Private Methods


        #endregion


        #region MonoBehaviour CallBacks

        private void Start()
        {
            characterInfoPanel.SetActive(false);
            buffInfoPanel.SetActive(false);
        }

        #endregion
    }
}
