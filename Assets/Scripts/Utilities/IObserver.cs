using UnityEngine;

namespace KWY
{
    public interface IObserver<T>
    {
        void OnNotify(T t);
    }

    public interface IObserver
    {
        void OnNotify();
    }
}
