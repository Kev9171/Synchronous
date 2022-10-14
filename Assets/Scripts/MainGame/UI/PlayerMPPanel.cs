using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        public void SetData(Sprite sprite, int mp)
        {
            playerImg.sprite = sprite;
            UpdateMP(0, 10);
        }

        public void UpdateUI(Player player)
        {
            UpdateMP(player.Mp, player.MaxMp);
        }

        private void UpdateMP(int mp, int maxMp)
        {
            mpLabel.text = mp.ToString();
            mpBar.value = mp / (float)10;
        }
    }
}
