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
        List<Button> volumeBtnList = new List<Button>();

        [SerializeField]
        List<Slider> volumeBarList = new List<Slider>();

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
        /// <summary>
        /// true if OFF(mute), false if ON
        /// </summary>
        List<bool> volumeMuteStatus = new List<bool>();

        bool IsModified = false;
        #endregion


        // temp
        public void SetData(Object o)
        {
            PlayingSettings settings = Resources.Load<PlayingSettings>("PlayingSettings");

            // btn 이미지 로드 및 변경
            // temp length 1
            for(int i=0; i<settings.Volume.Count; i++)
            {
                volumeMuteStatus.Add(settings.IsVolumeMute[i]);

                if (volumeMuteStatus[i])
                {
                    volumeBtnList[i].GetComponent<Image>().sprite = VolumeOffIcon;
                }

                // bar 위치 로드 및 변경
                volumeBarList[i].value = settings.Volume[i];
            }

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
            SetModified(true);

            // check range btn
            if (btn < 0 || btn >= volumeBtnList.Count) return;

            if (volumeMuteStatus[btn])
            {
                volumeBtnList[btn].GetComponent<Image>().sprite = VolumeOffIcon;
            }
            else
            {
                volumeBtnList[btn].GetComponent<Image>().sprite = VolumeOnIcon;
            }

            volumeMuteStatus[btn] = !(volumeMuteStatus[btn]);
        }

        public void OnClickApplySetting()
        {
            // data 저장 at Playing Settings
            Debug.Log("Apply Settings");

            PlayingSettings settings = Resources.Load<PlayingSettings>("PlayingSettings");

            settings.Volume[0] = volumeBarList[0].value;
            settings.Volume[1] = volumeBarList[1].value;

            settings.IsVolumeMute[0] = volumeMuteStatus[0];
            settings.IsVolumeMute[1] = volumeMuteStatus[1];

            SetModified(false);
        }

        public void OnVolumeValueChanged()
        {
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
