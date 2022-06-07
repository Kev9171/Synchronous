using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    [CreateAssetMenu(fileName = "UserManager", menuName = "Singletons/UserManager")]
    public class UserManager : SingletonScriptableObject<UserManager>
    {
        private string _accountId; // °íÀ¯°ª
        private Sprite _userIcon;
        private int _userLevel;
        private int _userId;

        public static string AccountId { get { return Instance._accountId; } }
        public static Sprite UserIcon { get { return Instance._userIcon; } }
        public static int UserLevel { get { return Instance._userLevel; } }
        public static int UserId { get { return Instance._userId; } }

        /// <summary>
        /// It should be called when login is successful; before using the instance; 
        /// </summary>
        /// <param name="userIcon">user profile icon</param>
        /// <param name="accountId">user unique id allocated from the server</param>
        /// <param name="userLevel">user level</param>
        /// <param name="userId">user id</param>
        public static void InitData(Sprite userIcon, string accountId, int userLevel, int userId)
        {
            Instance._accountId = accountId;
            Instance._userIcon = userIcon;
            Instance._userLevel = userLevel;
            Instance._userId = userId;
        }

    }
}
