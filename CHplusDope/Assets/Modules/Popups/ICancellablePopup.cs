using System;

namespace Popups
{
    public interface ICancellablePopup<T> : IPopup<T>
    {
        T Else(Action callback);
    }
    
    public interface ICancellablePopup<T,T2> : IPopup<T,T2>
    {
        T Else(Action<T2> callback);
    }
}