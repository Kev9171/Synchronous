using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class CharacterActionData
    {
        public object[] Actions { get; private set; }

        int idx = 0;

        public CharacterActionData()
        {
            Actions = new object[3];
        }

        public int Count { get { return idx >= 3 ? 3 : idx; } }


        public void ClearActions()
        {
            idx = 0;
        }
        public void AddAction(ActionType type, params object[] info )
        {
            Debug.LogFormat("Type: {0}, info: {1}", type, info);

            if (idx >= Actions.Length)
            {
                // 추가 처리 필요
                idx = 0;
            }

            if (type == ActionType.Move)
            {
                // 2개 (dx, dy)
                Actions[idx] = new object[] { type, (int)info[0], (int)info[1] };
            }
            else
            {
                // 2개 (sid, diection)
                Actions[idx] = new object[] { type, (int)info[0], (SkillDicection)info[1] };
            }
            idx++;
        }

        public void AddSkillAction(ActionType type, SID sid, SkillDicection dir, params object[] skillOps)
        {
            Debug.LogFormat("Type: {0}, SID: {1}, Direction: {2}, ops: {3}", type, sid, dir, skillOps);
            if (type != ActionType.Skill)
            {
                Debug.Log("Wrong method; This method is for skill");
            }
            if (idx >= Actions.Length)
            {
                // 추가 처리 필요
                idx = 0;
            }
            Actions[idx] = new object[] { type, (int)sid, dir, skillOps };
            idx++;
        }

        public void AddMoveAction(ActionType type, int dx, int dy, params object[] moveOps)
        {
            Debug.LogFormat("Type: {0}, dx: {1}, dy: {2}, ops: {3}", type, dx, dy, moveOps);
            if (type != ActionType.Move)
            {
                Debug.Log("Wrong method; This method is for move");
            }
            if (idx >= Actions.Length)
            {
                // 추가 처리 필요
                idx = 0;
            }
            Actions[idx] = new object[] { type, dx, dy, moveOps };
            idx++;
        }
    }
}
