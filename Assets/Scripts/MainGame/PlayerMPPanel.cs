using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class PlayerMPPanel : MonoBehaviour
    {
        // temp bar 구현 방법 찾아보기
        [SerializeField]
        private GameObject manaBar;

        [SerializeField]
        private Image playerImg;

        [SerializeField]
        private TMP_Text mpLabel;

        public void SetData(Sprite sprite, int mp)
        {
            playerImg.sprite = sprite;

            // bar
            UpdateMPBar();

            mpLabel.text = mp.ToString();
        }

        public void UpdateData(int mp)
        {
            mpLabel.text = mp.ToString();
            UpdateMPBar();
        }

        private void UpdateMPBar()
        {

        }
    }
}
