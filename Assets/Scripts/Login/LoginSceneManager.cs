using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KWY
{
    public class LoginSceneManager : MonoBehaviour
    {
        [SerializeField]
        GameObject InitialPanel;

        [SerializeField]
        GameObject LoginPanel;

        [SerializeField]
        GameObject JoinPanel;

        [SerializeField]
        GameObject SelectPanel;

        [SerializeField]
        GameObject ClickToStart;

        [SerializeField]
        Transform CanvasTransform;

        const string nextLevel = "StartScene";

        public void AfterLogin()
        {
            InitialPanel.SetActive(false);
            ClickToStart.SetActive(true);
        }

        #region Button Elements Callbacks

        public void OnClickLoginBtn()
        {
            SelectPanel.SetActive(false);
            LoginPanel.SetActive(true);
        }

        public void OnClickJoinBtn()
        {
            SelectPanel.SetActive(false);
            JoinPanel.SetActive(true);
        }

        public void OnClickInitialBackBtn()
        {
            LoginPanel.SetActive(false);
            JoinPanel.SetActive(false);
            SelectPanel.SetActive(true);
        }

        public void OnClickMainPanel()
        {
            SceneManager.LoadScene(nextLevel);
        }

        #endregion

        private void Start()
        {
            LoginPanel.SetActive(false);
            JoinPanel.SetActive(false);

            SelectPanel.SetActive(true);
        }
    }
}
