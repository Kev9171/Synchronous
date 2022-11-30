using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class PlayerMPPanel : MonoBehaviour
    {
        [SerializeField]
        private Image playerImg;

        [SerializeField]
        private TMP_Text mpLabel;

        [SerializeField]
        private Slider mpBar;

        Player player;

        public void SetData(Sprite sprite, Player player)
        {
            playerImg.sprite = sprite;
            this.player = player;
            UpdateMP(player.Mp);
        }

        public void UpdateUI()
        {
            UpdateMP(player.Mp);
        }

        private void UpdateMP(int mp)
        {
            int now = (int)mpBar.value;
            StartCoroutine(IEUpdateMp(mp - now, player.Mp));
        }

        IEnumerator IEUpdateMp(int dv, int desV)
        {
            float v = dv / 10f;
            for (float ft = 1f; ft >= 0; ft -= 0.1f)
            {
                mpBar.value += v;
                mpLabel.text = ((int)(mpBar.value)).ToString();
                yield return new WaitForSeconds(0.1f);
            }
            mpBar.value = desV;
            mpLabel.text = desV.ToString();
        }

        private void Start()
        {
            player = MainGameData.Instance.MyPlayer;
            mpBar.maxValue = player.MaxMp;
            mpBar.value = 0;
        }
    }
}
