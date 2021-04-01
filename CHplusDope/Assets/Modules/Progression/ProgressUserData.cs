namespace Progression
{
    public abstract class ProgressUserData<T>
    {
        public T Data { get; }

        public ProgressUserData(T data)
        {
            Data = data;
        }
    }
}