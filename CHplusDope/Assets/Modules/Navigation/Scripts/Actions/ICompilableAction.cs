namespace Navigation
{
    public interface ICompilableAction
    {
        void Compile(RawPath path, int ownIndex);
    }
}