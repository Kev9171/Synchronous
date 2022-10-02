using System.Collections;
using UnityEngine;

namespace KWY
{
    public class PlayerUIHandler : MonoBehaviour
    {
        [SerializeField]
        PlayerMPPanel playerMpPanel;

        public void Init()
        {

        }

        public void UpdatePlayerMpPanel(Player player)
        {
            Debug.Log($"UpdatePlayerMpPanel");
            playerMpPanel.UpdateUI(player);
        }
    }
}