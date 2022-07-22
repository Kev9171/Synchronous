using Photon.Pun;
using Photon.Realtime;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace KWY
{
    public class ListingRooms : MonoBehaviour
    {
        [SerializeField]
        RawImage MasterIcon;

        [SerializeField]
        TMP_Text RoomName;

        [SerializeField]
        TMP_Text RoomEx;

        [SerializeField]
        TMP_Text PlayerNum;

        public RoomInfo RoomInfo { get; private set; }

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            this.RoomInfo = roomInfo;
            RoomName.text = roomInfo.Name;
            // 설명 부분이 default로 없음 -> roominfo 상속해서 추가 생성 필요?... 뺴던가 몰?루...
            PlayerNum.text = roomInfo.PlayerCount.ToString() + "/" + roomInfo.MaxPlayers.ToString();
        }

        public void OnClicked()
        {
            GameObject photonObject = GameObject.Find("Photon");
            photonObject.GetComponent<ConnectPhoton>().JoinNamedRoom(RoomInfo.Name);
        }

        private void Start()
        {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            GetComponent<Image>().color = new Color(r, g, b, 0.5f);
        }
    }
}
