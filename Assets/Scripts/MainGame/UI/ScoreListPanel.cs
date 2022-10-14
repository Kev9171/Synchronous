using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class ScoreListPanel : MonoBehaviour
    {
        [SerializeField]
        TMP_Text contentLabel;

        [SerializeField]
        TMP_Text scoreText;

        public void SetData(string content, int score)
        {
            contentLabel.text = content;
            scoreText.text = score.ToString();
        }
    }
}
