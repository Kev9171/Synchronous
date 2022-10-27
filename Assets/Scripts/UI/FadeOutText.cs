using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using STask = System.Threading.Tasks.Task;

namespace UI
{
    public class FadeOutText : MonoBehaviour
    {
        [SerializeField]
        TMP_Text text;

        public void Init(string content)
        {
            text.text = content;
            StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut()
        {
            Color tc = text.color;
            for (float alpha = 1f; alpha >= 0f; alpha -= 0.01f)
            {
                tc.a = alpha;
                text.color = tc;
                yield return new WaitForSeconds(.01f);
            }
            Destroy(gameObject);
        }
    }
}


