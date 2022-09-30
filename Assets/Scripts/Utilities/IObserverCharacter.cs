using UnityEngine;

namespace KWY
{
    public interface IObserverCharacter<T>
    {
        void OnNotify(T t);
        void UpdateData(T t);
    }
}
