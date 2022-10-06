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

        public int ActionCount { get { return idx >= 3 ? 3 : idx; } }

        public void ClearActions()
        {
            idx = 0;
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
                // �߰� ó�� �ʿ�
                idx = 0;
            }

            int l = skillOps.Length;
            // ��ų �ߵ� ��ġ x, y -> 2�� �߰� ����
            if (l == 2)
            {
                Actions[idx] = new object[] { type, sid, dir, (int)skillOps[0], (int)skillOps[1] };
                idx++;
            }
            // �ܼ� ��ų (�߰� ��ǥ �ʿ� ����)
            else if (l == 0)
            {
                Actions[idx] = new object[] { type, sid, dir, 0, 0 };
                idx++;
            }
            // ����
            else
            {
                Debug.LogError("AddSkilAction Error");
            }
        }

        public void AddMoveAction(ActionType type, int dx, int dy, bool odd, params object[] moveOps)
        {
            Debug.LogFormat("Type: {0}, dx: {1}, dy: {2}, odd?: {3}, ops: {4}", type, dx, dy, odd, moveOps);
            if (type != ActionType.Move)
            {
                Debug.Log("Wrong method; This method is for move");
            }
            if (idx >= Actions.Length)
            {
                // �߰� ó�� �ʿ�
                idx = 0;
            }

            
            // ���� move�� �߰� �ɼ� ������ �� ����
            // TODO

            // odd ���� ���ΰ�...?.......
            // TODO


            Actions[idx] = new object[] { type, dx, dy, 0, 0 };
            idx++;
        }
    }
}
