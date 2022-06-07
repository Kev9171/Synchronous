using System.Collections;
using UnityEngine;

namespace KWY
{
    /// <summary>
    /// Character�� Action, side�� ���ڷ� �ް� �ش�Ǵ� ���̵忡 �׼� �����ֱ�
    /// </summary>
    public class ShowNowAction : MonoBehaviour
    {
        [SerializeField]
        private GameObject leftSkillLinePanel;
        [SerializeField]
        private GameObject rightSkillLinePanel;
        [SerializeField]
        private GameObject leftActionPiecePrefab;
        [SerializeField]
        private GameObject rightActionPiecePrefab;

        [Tooltip("True�� üũ�ϰ� �����ϸ� �׽�Ʈ�� �׼��� ��� ������")]
        public bool execTestYieldAction = false;

        public ShowNowAction(GameObject left, GameObject right)
        {
            this.leftSkillLinePanel = left;
            this.rightSkillLinePanel = right;
        }

        public void ShowAction(CharacterBase cb, ActionBase ab, bool side)
        {
            if (!side)
            {
                Instantiate(leftActionPiecePrefab, leftSkillLinePanel.transform).GetComponent<ActionPiece>().SetData(cb.icon, ab.name);
            }
            else
            {
                Instantiate(rightActionPiecePrefab, rightSkillLinePanel.transform).GetComponent<ActionPiece>().SetData(cb.icon, ab.name);
            }

        }

        private void Start()
        {
            if (execTestYieldAction)
                StartCoroutine("CallTest", 0.7f);
        }

        IEnumerator CallTest(float delaytime)
        {
            int t = Random.Range(0, 2);
            ShowAction(CharaManager.GetData(CID.Flappy), SkillManager.GetData(SID.FireBall), t==0);
            yield return new WaitForSeconds(delaytime);
            if (execTestYieldAction)
                StartCoroutine("CallTest", 0.7f);
        }
    }
}
