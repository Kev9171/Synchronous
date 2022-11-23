using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using KWY;

namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class CharacterHpBar : MonoBehaviour
    {
        [SerializeField]
        Slider hpBar;

        Transform followingTarget;

        Character target;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
