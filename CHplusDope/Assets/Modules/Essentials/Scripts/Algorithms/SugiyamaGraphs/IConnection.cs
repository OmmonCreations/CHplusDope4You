namespace Essentials.Algorithms.SugiyamaGraphs
{
    public interface IConnection<T>
    {
        T From { get; }
        T To { get; }
    }
}