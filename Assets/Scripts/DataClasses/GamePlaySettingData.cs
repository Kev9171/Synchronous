using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace KWY
{
    [Serializable]
    public class GamePlaySettingData
    {
        public bool BGM_Mute;
        public bool SE_Mute;

        public float BGM_Volume;
        public float SE_Volume;

        public void SetData(bool bgm_mute, bool se_mute, float bgm_volume, float se_volume)
        {
            BGM_Mute = bgm_mute;
            SE_Mute = se_mute;
            BGM_Volume = bgm_volume;
            SE_Volume = se_volume;
        }

        public void SetData(GamePlaySettingData data)
        {
            BGM_Mute = data.BGM_Mute;
            SE_Mute = data.SE_Mute;
            BGM_Volume = data.BGM_Volume;
            SE_Volume = data.SE_Volume;
        }

        public override string ToString()
        {
            return String.Format("BGM_Mute: {0}, SE_Mute: {1}, BGM_Volume: {2}, SE_Volume: {3}", BGM_Mute, SE_Mute, BGM_Volume, SE_Volume);
        }
    }
}