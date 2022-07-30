using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace KWY
{
    public class GameLobbyHelpPopup : MonoBehaviour, IInstantiatableUI
    {
        [SerializeField]
        TMP_Text TitleText;

        [SerializeField]
        Button CloseBtn;

        public void SetData(Object o)
        {
            ;
        }

        public void Init()
        {
            ;
        }

        public void OnClickClose()
        {
            Destroy(gameObject);
        }
    }
}
