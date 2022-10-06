using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KWY
{
    public class ActionData
    {
        Dictionary<int, object[]> _data;

        //Dictionary<int, object[]> _data;

        public Dictionary<int, object[]> Data { get { return _data; } }

        public ActionData()
        {
            _data = new Dictionary<int, object[]>();
        }

        // Serialize ����� method
        public ActionData(Dictionary<int, object[]> data)
        {
            _data = data;
        }

        public static ActionData CreateActionData(Dictionary<int, CharacterActionData> data)
        {
            ActionData ad = new ActionData();
            

            foreach(int id in data.Keys)
            {
                int idx = 0;
                object[] value = new object[data[id].ActionCount];
                int time = 0;
                foreach (object[] action in data[id].Actions)
                {
                    

                    ActionType type = (ActionType)action[0];
                    object[] timeData;

                    // time ���ϱ�
                    if (type == ActionType.Skill)
                    {
                        time += SkillManager.GetData((SID)action[1]).triggerTime;

                        // ���� ���̴� 6���� ����
                        timeData = new object[] { time, type, (SID)action[1], (SkillDicection)action[2], action[3], action[4] };
                    }
                    // move.triggerTime = 0
                    else
                    {
                        timeData = new object[] { time, type, (int)action[1], (int)action[2], action[3], action[4] };
                    }

                    value[idx++] = timeData;

                    // time shift
                    if (type == ActionType.Skill)
                    {
                        time += SkillManager.GetData((SID)action[1]).castingTime;
                    }
                    else
                    {
                        time += 2; // Move ���
                    }
                }

                ad.Data.Add(id, value);
            }

            return ad;
        }

        public override string ToString()
        {
            string t = "[";

            foreach(int id in _data.Keys)
            {
                t += $"id: {id} [";
                foreach(object[] o in _data[id])
                {
                    if ((ActionType)(o[1]) == ActionType.Move)
                    {
                        t += $"time: {(int)o[0]}, type: MOVE, dx: {o[2]}, dy: {o[3]}, ex0: {o[4]}, ex1: {o[5]} / ";
                    }
                    else
                    {
                        t += $"time: {(int)o[0]}, type: SKILL, sid: {o[2]}, skill dir: {o[3]}, s_x: {o[4]}, s_y: {o[5]} / ";
                    }

                }
                t += "]";
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
