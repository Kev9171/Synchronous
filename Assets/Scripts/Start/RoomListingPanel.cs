using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KWY
{
    public class RoomListingPanel : MonoBehaviourPunCallbacks, IInstantiatableUI
    {
        [SerializeField]
        Transform ListConentTransform;

        [SerializeField]
        TMP_Text LoadingText;

        [SerializeField]
        TMP_Text NoRoomText;

        [SerializeField]
        Animator ReloadBtnAnimator;

        private List<ListingRooms> _listings = new List<ListingRooms>();
        public void SetData(Object o)
        {
            ;
        }

        public void Init()
        {
            ;
        }

        // 수동 새로고침
        private void LoadRoomList()
        {
            LoadingText.gameObject.SetActive(true);

            PhotonNetwork.GetCustomRoomList(new TypedLobby(ConstantValue.TYPPED_LOBBY_SQL_NAME, LobbyType.SqlLobby), ConstantValue.REFRESH_ROOM_SQL);

            // photon2에는 방 리스트를 가져오는 함수가 없음(오직 callback으로만 있음...)
            // 다른 유저가 접근 불가능한 방을 잠깐 들어갔다가 나오는 식을 이용... (비효율...)
            /*RoomOptions roomOptions = new RoomOptions()
            {
                IsVisible = false,
                MaxPlayers = 1
            };

            PhotonNetwork.JoinOrCreateRoom(DefaultRoomName, roomOptions, TypedLobby.Default);*/
        }

        #region Override Callbacks about Room Listing

        // 일단 callback으로 새로운 방 생겼을 때 자동으로 갱신 되도록
        // 만약 룸이 매우 많아 지게 되면 수동으로 하는 새로고침 버튼 추가해서 할 필요성 있음
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            // 현재: 모든 방을 삭제후 새로 받은 리스트로 방 리스트를 모두 instantiate 하고 있음...
            // 많아지면 느려질 수 있음 -> 어떻해 하죠...

            foreach(ListingRooms lr in _listings)
            {
                Destroy(lr.gameObject);
            }
            _listings.Clear();

            foreach (RoomInfo info in roomList)
            {
                GameObject g = Instantiate(
                            Resources.Load("Prefabs/UI/ListingElements/RoomListing",
                            typeof(GameObject)), ListConentTransform) as GameObject;

                ListingRooms listing = g.GetComponent<ListingRooms>();

                if (listing != null)
                {
                    listing.SetRoomInfo(info);
                    _listings.Add(listing);
                }
            }


            /*foreach (RoomInfo info in roomList)
            {
                // 룸 리스트에서 삭제
                if (info.RemovedFromList)
                {
                    int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                    if (index != -1)
                    {
                        Destroy(_listings[index].gameObject);
                        _listings.RemoveAt(index);
                    }
                }
                // 룸 리스트에 추가
                else
                {
                    int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                    if (index == -1)
                    {
                        GameObject g = Instantiate(
                            Resources.Load("Prefabs/UI/ListingElements/RoomListing",
                            typeof(GameObject)), ListConentTransform) as GameObject;

                        ListingRooms listing = g.GetComponent<ListingRooms>();

                        if (listing != null)
                        {
                            listing.SetRoomInfo(info);
                            _listings.Add(listing);
                        }
                    }
                    else
                    {
                        // _listings[index].
                    }
                }
            }*/

            if (roomList.Count == 0)
            {
                NoRoomText.gameObject.SetActive(true);
            }
            else
            {
                NoRoomText.gameObject.SetActive(false);
            }

            LoadingText.gameObject.SetActive(false);
            ReloadBtnAnimator.speed = 0.0f;
        }

        #endregion

        public void ReLoadRooms()
        {
            ReloadBtnAnimator.speed = 1.0f;
            LoadRoomList();
        }

        /*IEnumerator RefreshRooms()
        {
            LoadRoomList();
            yield return new WaitForSeconds(5);
            StartCoroutine(RefreshRooms());
        }*/

        #region Button Callbacks

        public void OnClickClose()
        {
            Destroy(gameObject);
        }

        #endregion


        #region MonoBehaviourCallbacks

        private void Start()
        {
            //StartCoroutine(RefreshRooms());
            ReLoadRooms();
        }

        #endregion
    }
}
