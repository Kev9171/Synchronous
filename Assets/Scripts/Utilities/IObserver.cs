using UnityEngine;

namespace KWY
{
    public interface IObserver<T>
    {
        void OnNotify(T t);
        void UpdateData(T t);
    }

    public interface IObserver
    {
        void OnNotify();
        void UpdateData();
    }
}
