using TMPro;
using UnityEngine;

using UnityEngine.UI;

using System.Collections.Generic;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class SettingPanel : MonoBehaviour, IInstantiatableUI
    {
        #region UI Elements
        [SerializeField]
        Button BGMMuteBtn;

        [SerializeField]
        Button SEMuteBtn;

        [SerializeField]
        Slider BGMBar;

        [SerializeField]
        Slider SEBar;

        [SerializeField]
        GameObject applyBtn;

        [SerializeField]
        GameObject closePopup;

        #endregion

        #region Materials
        [SerializeField]
        Sprite VolumeOnIcon;

        [SerializeField]
        Sprite VolumeOffIcon;
        #endregion

        #region Private Fields

        GamePlaySettingData tempData = new GamePlaySettingData();

        bool IsModified = false;
        #endregion


        // temp
        public void SetData(Object o)
        {
            SettingManager settingManager = SettingManager.Instance;

            // deep copy
            tempData.SetData(settingManager.gameSettings);

            BGMMuteBtn.GetComponent<Image>().sprite = tempData.BGM_Mute ? VolumeOffIcon : VolumeOnIcon;
            SEMuteBtn.GetComponent<Image>().sprite = tempData.SE_Mute ? VolumeOffIcon : VolumeOnIcon;
            BGMBar.value = tempData.BGM_Volume;
            SEBar.value = tempData.SE_Volume;

            SetModified(false);
        }


        public void Init()
        {
            //
        }

        public void OnClickClose()
        {
            if (IsModified)
            {
                closePopup.SetActive(true);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OnClickVolumeBtn(int btn)
        {
            switch(btn)
            {
                case 0:
                    // bgm
                    if (tempData.BGM_Mute)
                    {
                        BGMMuteBtn.GetComponent<Image>().sprite = VolumeOnIcon;
                    }
                    else
                    {
                        BGMMuteBtn.GetComponent<Image>().sprite = VolumeOffIcon;
                    }
                    tempData.BGM_Mute = !tempData.BGM_Mute;
                    break;
                case 1:
                    // se
                    if (tempData.SE_Mute)
                    {
                        SEMuteBtn.GetComponent<Image>().sprite = VolumeOnIcon;
                    }
                    else
                    {
                        SEMuteBtn.GetComponent<Image>().sprite = VolumeOffIcon;
                    }
                    tempData.SE_Mute = !tempData.SE_Mute;
                    break;
                default:
                    break;
            }

            SetModified(true);
        }

        public void OnClickApplySetting()
        {
            SettingManager settingManager = SettingManager.Instance;

            settingManager.gameSettings.SetData(tempData);
            settingManager.SaveGameSettings();

            SetModified(false);
        }

        public void OnVolumeValueChanged(int bar)
        {
            switch(bar)
            {
                case 0:
                    // bgm
                    tempData.BGM_Volume = BGMBar.value;
                    break;
                case 1:
                    // se
                    tempData.SE_Volume = SEBar.value;
                    break;
                default:
                    break;
            }
            SetModified(true);
        }

        public void ClosePopupOnOkBtnClicked()
        {
            Destroy(gameObject);
        }

        public void CLosePopupOnCancelBtnClicked()
        {
            closePopup.SetActive(false);
        }
        
        private void SetModified(bool modified)
        {
            if (modified)
            {
                IsModified = true;
                applyBtn.GetComponent<Image>().color = Color.white;
                applyBtn.GetComponent<Button>().interactable = true;
            }
            else
            {
                IsModified = false;
                applyBtn.GetComponent<Image>().color = Color.gray;
                applyBtn.GetComponent<Button>().interactable = false;
            }
        }

        private void Start()
        {
            closePopup.SetActive(false);
        }
    }
}
