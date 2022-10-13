﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KWY
{
    public class Player : MonoBehaviour, ISubject<Player>
    {
        public List<IObserver<Player>> Observers
        {
            get;
            private set;
        } = new List<IObserver<Player>>();

        public int Mp
        {
            get;
            private set;
        } = 0;

        public Image playerImg
        {
            get;
            private set;
        }

        public readonly int MaxMp = 10;

        public void InitData(Image icon, int initialMp)
        {
            Mp = initialMp;
            playerImg = icon;
        }

        public void AddMp(int value)
        {
            Mp += value;

            NotifyObservers();
        }

        public void SubMp(int value)
        {
            Mp -= value;

            NotifyObservers();
        }

        #region ISubject Methods
        public void AddObserver(IObserver<Player> o)
        {
            if (Observers.IndexOf(o) < 0)
            {
                Observers.Add(o);
            }
            else
            {
                Debug.LogWarning($"The observer already exists in list: {o}");
            }
        }

        public void NotifyObservers()
        {
            foreach (IObserver<Player> o in Observers)
            {
                o.OnNotify(this);
            }
        }

        public void RemoveAllObservers()
        {
            Observers.Clear();
        }

        public void RemoveObserver(IObserver<Player> o)
        {
            int idx = Observers.IndexOf(o);
            if (idx >= 0)
            {
                Observers.RemoveAt(idx); // O(n)
            }
            else
            {
                Debug.LogError($"Can not remove the observer; It does not exist in list: {o}");
            }
        }
        #endregion
    }
}