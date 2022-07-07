using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KWY
{
    [CreateAssetMenu(fileName = "PlayingSettings", menuName = "PlayingSettings")]
    public class PlayingSettings : ScriptableObject
    {
        #region need-not-to-hide fields

        [Tooltip("0: BGM, 1: SE; float")]
        [SerializeField]
        public List<float> Volume = new List<float>();

        [Tooltip("0: BGM, 1: SE; True if mute")]
        [SerializeField]
        public List<bool> IsVolumeMute = new List<bool>();

        [Tooltip("�׸� ����(color)")]
        [SerializeField]
        public Color ThemeColor;

        #endregion
    }
}
