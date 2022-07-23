using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class InitialManager : MonoBehaviour
    {
        [SerializeField]
        GameObject MainPanel;

        [SerializeField]
        GameObject InitialPanel;

        [SerializeField]
        GameObject LoginPanel;

        [SerializeField]
        GameObject JoinPanel;

        [SerializeField]
        GameObject SelectPanel;

        #region Button Elements Callbacks

        public void OnClickLoginBtn()
        {
            SelectPanel.SetActive(false);
            LoginPanel.SetActive(true);
        }

        public void OnClickJoinBtn()
        {
            Debug.Log("OnClickJoinBtn");
            SelectPanel.SetActive(false);
            JoinPanel.SetActive(true);
        }

        public void OnClickInitialBackBtn()
        {
            LoginPanel.SetActive(false);
            JoinPanel.SetActive(false);
            SelectPanel.SetActive(true);
        }

        #endregion

        private void Start()
        {
            MainPanel.SetActive(false);
            LoginPanel.SetActive(false);
            JoinPanel.SetActive(false);

            InitialPanel.SetActive(true);

            SelectPanel.SetActive(true);
        }
    }
}
