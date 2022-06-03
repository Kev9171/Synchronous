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

        public void SetPlayerImage(Sprite sprite)
        {
            playerImg.sprite = sprite;
        }

        public void SetData(Sprite sprite, int mp)
        {
            playerImg.sprite = sprite;

            // bar

            mpLabel.text = mp.ToString();
        }
    }
}
