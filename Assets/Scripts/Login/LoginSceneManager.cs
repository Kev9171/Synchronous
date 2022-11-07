using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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

        [SerializeField]
        GameObject MainPanel;

        const string nextLevel = "StartScene";

        public void AfterLogin()
        {
            InitialPanel.SetActive(false);
            StartCoroutine(TextFlicker());

            MainPanel.GetComponent<EventTrigger>().enabled = true;
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
            ClickToStart.SetActive(false);
            LoginPanel.SetActive(false);
            JoinPanel.SetActive(false);

            SelectPanel.SetActive(true);

            MainPanel.GetComponent<EventTrigger>().enabled = false;
        }

        private IEnumerator TextFlicker()
        {
            while (true)
            {
                ClickToStart.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                ClickToStart.SetActive(false);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
