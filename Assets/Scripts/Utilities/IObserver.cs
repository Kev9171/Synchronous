using UnityEngine;
using System.Collections.Generic;

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
