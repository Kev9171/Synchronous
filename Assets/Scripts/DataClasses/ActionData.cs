using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KWY
{
    public class ActionData
    {
        Dictionary<int, object[]> _data;

        public Dictionary<int, object[]> Data { get { return _data; } set { _data = value; } }

        public ActionData()
        {
            _data = new Dictionary<int, object[]>();
        }

        // Serialize ����� method
        public ActionData(Dictionary<int, object[]> data)
        {
            _data = data;
        }

        public static ActionData CreateActionData(Dictionary<CID, CharacterActionData> data)
        {
            ActionData ad = new ActionData();

            foreach(CID c in data.Keys)
            {
                int time = 0;
                int cid = (PhotonNetwork.IsMasterClient) ? (int)c : (int)c + 100;
                foreach (object[] action in data[c].Actions)
                {
                    ActionType type = (ActionType)action[0];
                    object[] timeData;

                    // time ���ϱ�
                    if (type == ActionType.Skill)
                    {
                        
                        time += SkillManager.GetData((SID)action[1]).triggerTime;

                        if (action.Length >= 4)
                        {
                            timeData = new object[] { time, type, (SID)action[1], (SkillDicection)action[2], action[3]};
                        }
                        else
                        {
                            timeData = new object[] { time, type, (SID)action[1], (SkillDicection)action[2] };
                        }
                    }
                    // move.triggerTime = 0
                    else
                    {
                        if (action.Length >= 4)
                        {
                            timeData = new object[] { time, type, (int)action[1], (int)action[2], action[3] };
                        }
                        else
                        {
                            timeData = new object[] { time, type, (int)action[1], (int)action[2] };
                        }
                    }

                    //�̹� �ش� time�� �����Ͱ� ���� ��� ���� �����Ϳ� �ϳ� �߰�
                    if (ad.Data.ContainsKey(cid))
                    {
                        object[] tData = new object[ad.Data[cid].Length + 1];
                        int idx = 0;
                        foreach (object[] t in ad.Data[cid])
                        {
                            tData[idx++] = t;
                        }
                        tData[idx] = timeData;

                        ad.Data[cid] = tData;
                    }
                    else
                    {
                        ad.Data.Add(cid, new object[] { timeData });
                    }
                    //ad.Data.Add((CID)cid, new object[] { timeData });

                    if (type == ActionType.Skill)
                    {
                        time += SkillManager.GetData((SID)action[1]).castingTime;
                    }
                    else
                    {
                        time += 2; // Move ���
                    }
                }
            }

            return ad;
        }

        public override string ToString()
        {
            string t = "[";

            foreach(int ti in _data.Keys)
            {
                t += string.Format("t: {0}", ti);
                foreach(object[] d in _data[ti])
                {
                    t += string.Format("<{0}, {1}, {2}, {3}> / ", d[0], (ActionType) d[1], d[2], d[3]);
                }
            }
            t += "]";

            return t;
        }

        #region For Photon CustomType
        // byte ������ serialize �ϴ� ���� ��� ������.....�ФФ� -> Dictionary<Object, Object>�� �ϴ�...
        // https://doc.photonengine.com/en-us/realtime/current/reference/serialization-in-photon

        #endregion
    }
}
