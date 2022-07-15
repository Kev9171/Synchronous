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
            UpdateMP(mp);
        }

        public void UpdateUI()
        {
            int mp = GameObject.Find("GameData").GetComponent<MainGameData>().PlayerMp;
            UpdateMP(mp);
        }

        private void UpdateMP(int mp)
        {
            mpLabel.text = mp.ToString();
            mpBar.value = mp / (float)10;
        }
    }
}
