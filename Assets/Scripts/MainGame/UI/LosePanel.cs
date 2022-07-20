using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;

using TMPro;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class LosePanel : MonoBehaviour, IInstantiatableUI
    {
        [SerializeField]
        Button okBtn;

        // юс╫ц╥н Object
        public void SetData(Object o)
        {

        }

        public void Init()
        {
            //
        }

        public void OnClickClose()
        {
            // Leave the room 
            if (PhotonNetwork.LeaveRoom(false))
            {
                Debug.Log("Leave the room...");
            }
            else
            {
                Debug.Log("Can not leave the room");
            }

            // load the start scene
            SceneManager.LoadScene("StartScene");
        }

        public void OnOkBtnClick()
        {
            Debug.Log("LosePanel - OnOkBtnClick");
            OnClickClose();
        }
    }
}
