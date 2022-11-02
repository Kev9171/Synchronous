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
        Image blockImage;
        [SerializeField]
        Image charaIcon;
        [SerializeField]
        TMP_Text skillNameLabel;

        [SerializeField]
        Sprite moveSprite;

        [SerializeField]
        Sprite skillSprite;

        private float activeDuration;

        public void SetDataAndStart(Sprite icon, string skillName, float duration, ActionType type)
        {
            if (type == ActionType.Move)
            {
                blockImage.sprite = moveSprite;
            }
            else
            {
                blockImage.sprite = skillSprite;
            }

            charaIcon.sprite = icon;
            skillNameLabel.text = skillName;
            activeDuration = duration;

            Destroy(gameObject, duration);

            //StartCoroutine("IEStartShowing");
        }

        /*IEnumerator IEStartShowing()
        {
            yield return new WaitForSeconds(activeDuration);
            Destroy(gameObject);
        }*/
    }
}
