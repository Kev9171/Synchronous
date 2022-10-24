using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using KWY;

namespace PickScene
{
    public class PickData : MonoBehaviour
    {
        public List<CharaData> Data
        {
            get;
            private set;
        } = new List<CharaData>();

        public void AddData(CID cid, Vector3Int cellV, Team team)
        {
            Data.Add(new CharaData(cid, cellV.x, cellV.y, team));
        }

        [PunRPC]
        public void SendDataToMasterRPC()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return;
            }

            PhotonView photonView = PhotonView.Get(this);
            foreach(CharaData d in Data)
            {
                photonView.RPC("ReceiveDataFromClientRPC", RpcTarget.Others, (int)d.cid, d.loc.x, d.loc.y, (int)d.team);
            }
        }

        public void RequestDataToMaster()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            PhotonView.Get(this).RPC("SendDataToMasterRPC", RpcTarget.Others);
        }

        [PunRPC]
        public void ReceiveDataFromClientRPC(int cid, int x, int y, int team)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            Data.Add(new CharaData((CID)cid, x, y, (Team)team));
        }

        public override string ToString()
        {
            string t = "";

            foreach(CharaData d in Data)
            {
                t += $"[cid:{d.cid}, cellV: {d.loc}, team: {d.team}], ";
            }

            return t;
        }
    }
}
