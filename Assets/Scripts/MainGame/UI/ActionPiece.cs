using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

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
        SLogicData logicData;

        private float activeDuration;

        public void SetDataAndStart(Sprite icon, string skillName, float duration)
        {
            charaIcon.sprite = icon;
            skillNameLabel.text = skillName;
            activeDuration = duration;

            StartCoroutine("IEStartShowing");
        }

        IEnumerator IEStartShowing()
        {
            yield return new WaitForSeconds(activeDuration);
            Destroy(gameObject);
        }
    }
}
