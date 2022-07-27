using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace KWY
{
    public class UserProfilePanel : MonoBehaviour
    {
        [SerializeField]
        Image userIcon;

        [SerializeField]
        TMP_Text userNameText;

        public void SetData(Sprite icon, string userName)
        {
            userIcon.sprite = icon;
            userNameText.text = userName;
        }

        public void LoadNowUser()
        {
            userIcon.sprite = UserManager.UserIcon;
            userNameText.text = UserManager.AccountId;
        }
    }
}
