using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace KWY
{
    public class CharacterHpBarController : MonoBehaviour
    {
        [SerializeField]
        GameObject hpBarPrefab;

        [SerializeField]
        Transform canvasTransform;

        public static CharacterHpBarController Instance;

        // key: slider
        // value: character
        Dictionary<Character, GameObject> hpBars = new Dictionary<Character, GameObject>();

        new Camera camera;

        public void AddBar(Character targetCharacter)
        {
            GameObject g = Instantiate(hpBarPrefab, canvasTransform);
            if (g.TryGetComponent(out Slider s))
            {
                s.maxValue = targetCharacter.MaxHp;
                s.value = s.maxValue;
            }
            else
            {
                // error
                Debug.Log($"Can not find component: Slider at character[{targetCharacter.Pc.Id}]");
                return;
            }
            hpBars.Add(targetCharacter, g);
        }

        public void HideBar(Character character)
        {
            if (hpBars.TryGetValue(character, out GameObject g))
            {
                g.SetActive(false);
            }
        }

        public void UpdateHp(Character character)
        {
            if (hpBars[character].activeSelf)
            {
                int now = (int)hpBars[character].GetComponent<Slider>().value;
                int v = character.Hp - now;
                if (v == 0) { return; }
                StartCoroutine(IEUpdateHp(hpBars[character].GetComponent<Slider>(), v, character.Hp));
            }
        }

        IEnumerator IEUpdateHp(Slider s, int dv, int fValue)
        {
            float v = dv / 10f;
            for (float ft = 1f; ft >= 0; ft -= 0.1f)
            {
                s.value += v;
                yield return new WaitForSeconds(0.1f);
            }
            s.value = fValue;

            if (s.value <= 0)
            {
                s.gameObject.SetActive(false);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if (!PhotonNetwork.InRoom)
            {
                return;
            }

            foreach (Character c in hpBars.Keys)
            {
                if (hpBars[c].activeSelf)
                {
                    hpBars[c].transform.position = camera.WorldToScreenPoint(c.gameObject.transform.position + new Vector3(0, c.HpBarRelativePosY, 0));
                }
            }
        }
    }
}


