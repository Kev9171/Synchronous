using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using KWY;
using DebugUtil;

namespace PickScene
{
    public class PickData : MonoBehaviour
    {
        public static PickData Instance;

        public List<CharaDataForPick> Data
        {
            get;
            private set;
        } = new List<CharaDataForPick>();

        public void AddData(CID cid, int x, int y, Team team)
        {
            Data.Add(new CharaDataForPick(cid, x, y, team));
        }

        public void AddData(CID cid, Vector3Int cellV, Team team)
        {
            Data.Add(new CharaDataForPick(cid, cellV.x, cellV.y, team));
        }

        public void SendDataToMaster()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return;
            }
            foreach(CharaDataForPick d in Data)
            {
                MainGameData.Instance.photonView.RPC("ReceiveDataFromClientRPC", RpcTarget.MasterClient, (int)d.cid, d.loc.x, d.loc.y, (int)d.team);
            }
        }        

        public override string ToString()
        {
            string t = "";

            foreach(CharaDataForPick d in Data)
            {
                t += $"[cid:{d.cid}, cellV: {d.loc}, team: {d.team}], ";
            }

            return t;
        }

        private void Start()
        {
            Instance = this;
        }
    }
}
