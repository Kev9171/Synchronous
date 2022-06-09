using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class ActionPiece : MonoBehaviour
    {
        [SerializeField]
        Image charaIcon;
        [SerializeField]
        TMP_Text skillNameLabel;
        [SerializeField]
        LogicData logicData;

        private float activeDuraction = 2f;
        private float time = 0.0f;

        public void SetData(Sprite icon, string skillName)
        {
            charaIcon.sprite = icon;
            skillNameLabel.text = skillName;
        }

        private void Start()
        {
            activeDuraction = logicData.actionLogShowingTime;
        }

        #region MonoBehaviour CallBacks
        private void Update()
        {
            time += Time.deltaTime;

            if (time > activeDuraction)
            {
                Destroy(gameObject);
                //PhotonNetwork.Destroy(gameObject);
            }
        }
        #endregion
    }
}
