using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;

namespace KWY
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        TMP_Text TimeText;

        public float StartTime
        {
            set;
            get;
        } = -1;

        public bool IsRunning
        {
            set;
            get;
        } = false;

        public UnityAction TimeOutCallback
        {
            set;
            get;
        } = null;

        private float timeRemaining = -1f;

        public void InitTimer(float startTime, UnityAction timeOutCallback)
        {
            this.StartTime = startTime;
            this.TimeOutCallback = timeOutCallback;
            this.timeRemaining = startTime;
        }

        public void InitTimer(float startTime, UnityAction timeOutCallback, TMP_Text textObject)
        {
            this.StartTime = startTime;
            this.TimeOutCallback = timeOutCallback;
            this.TimeText = textObject;
            this.timeRemaining = startTime;
        }

        public void StartTimer()
        {
            IsRunning = true;
        }

        public void StopTimer()
        {
            IsRunning = false;
            timeRemaining = StartTime;
        }

        public void PauseTimer()
        {
            IsRunning = false;
        }

        public void Update()
        {
            if (IsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    if (TimeText != null)
                        TimeText.text = Mathf.Ceil(timeRemaining).ToString();
                }
                else
                {
                    timeRemaining = 0;
                    IsRunning = false;

                    TimeOutCallback();
                }
            }
        }
    }
}
