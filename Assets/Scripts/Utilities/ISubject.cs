using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public interface ISubject
    {
        void AddObserver(IObserverCharacter<Character> o);
        void RemoveObserver(IObserverCharacter<Character> o);
        void NotifyObservers();
        void RemoveAllObservers();
    }
}
