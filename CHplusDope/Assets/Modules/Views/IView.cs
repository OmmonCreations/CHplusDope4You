namespace Views
{
    public interface IView : IGenericView
    {
        void Open();
    }

    public interface IView<in T> : IGenericView
    {
        void Open(T data);
    }

    public interface IView<in T1, in T2> : IGenericView
    {
        void Open(T1 a, T2 b);
    }

    public interface IView<in T1, in T2, in T3> : IGenericView
    {
        void Open(T1 a, T2 b, T3 c);
    }

    public interface IView<in T1, in T2, in T3, in T4> : IGenericView
    {
        void Open(T1 a, T2 b, T3 c, T4 d);
    }

    public interface IView<in T1, in T2, in T3, in T4, in T5> : IGenericView
    {
        void Open(T1 a, T2 b, T3 c, T4 d, T5 e);
    }

    public interface IView<in T1, in T2, in T3, in T4, in T5, in T6> : IGenericView
    {
        void Open(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f);
    }
}