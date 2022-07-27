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

        // ���� ���ΰ�ħ
        private void LoadRoomList()
        {
            LoadingText.gameObject.SetActive(true);

            PhotonNetwork.GetCustomRoomList(new TypedLobby(ConstantValue.TYPPED_LOBBY_SQL_NAME, LobbyType.SqlLobby), ConstantValue.REFRESH_ROOM_SQL);

            // photon2���� �� ����Ʈ�� �������� �Լ��� ����(���� callback���θ� ����...)
            // �ٸ� ������ ���� �Ұ����� ���� ��� ���ٰ� ������ ���� �̿�... (��ȿ��...)
            /*RoomOptions roomOptions = new RoomOptions()
            {
                IsVisible = false,
                MaxPlayers = 1
            };

            PhotonNetwork.JoinOrCreateRoom(DefaultRoomName, roomOptions, TypedLobby.Default);*/
        }

        #region Override Callbacks about Room Listing

        // �ϴ� callback���� ���ο� �� ������ �� �ڵ����� ���� �ǵ���
        // ���� ���� �ſ� ���� ���� �Ǹ� �������� �ϴ� ���ΰ�ħ ��ư �߰��ؼ� �� �ʿ伺 ����
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            // ����: ��� ���� ������ ���� ���� ����Ʈ�� �� ����Ʈ�� ��� instantiate �ϰ� ����...
            // �������� ������ �� ���� -> ��� ����...

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
                // �� ����Ʈ���� ����
                if (info.RemovedFromList)
                {
                    int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                    if (index != -1)
                    {
                        Destroy(_listings[index].gameObject);
                        _listings.RemoveAt(index);
                    }
                }
                // �� ����Ʈ�� �߰�
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
