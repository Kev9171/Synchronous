using System.Collections;
using UnityEngine;

namespace KWY
{
    /// <summary>
    /// Character와 Action, side를 인자로 받고 해당되는 사이드에 액션 보여주기
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

        [Tooltip("True로 체크하고 시작하면 테스트로 액션이 계속 생성됨")]
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
