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
            if (icon)
            {
                userIcon.sprite = icon;
            }
            
            userNameText.text = userName;
        }

        public void LoadNowUser()
        {
            if (UserManager.UserIcon)
            {
                userIcon.sprite = UserManager.UserIcon;
            }
            userNameText.text = UserManager.UserName;
        }
    }
}
