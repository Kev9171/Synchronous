using UnityEngine;
using UnityEngine.UI;

namespace KWY
{
    public class UIControlGameEnd : MonoBehaviour
    {
        #region UI Elements

        public GameObject WinnerPanel;
        public GameObject LoserPanel;

        #endregion


        #region Public Methods

        public void ShowWinnerPanel()
        {
            WinnerPanel.SetActive(true);
        }

        public void ShowLoserPanel()
        {
            LoserPanel.SetActive(true);
        }

        #endregion

        #region MonoBehaviours CallBacks
        private void Awake()
        {
            WinnerPanel.SetActive(false);
            LoserPanel.SetActive(false);
        }
        #endregion
    }
}
