using System;

namespace Popups
{
    public interface IPopup<T>
    {
        T Then(Action callback);
    }

    public interface IPopup<T,T2>
    {
        T Then(Action<T2> callback);
    }
}