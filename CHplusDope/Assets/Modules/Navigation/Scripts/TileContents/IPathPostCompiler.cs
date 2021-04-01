namespace Navigation
{
    public interface IPathPostCompiler
    {
        void PostCompile(RawPath path, int ownIndex);
    }
}