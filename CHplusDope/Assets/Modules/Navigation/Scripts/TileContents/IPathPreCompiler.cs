namespace Navigation
{
    public interface IPathPreCompiler
    {
        void PreCompile(RawPath path, int ownIndex);
    }
}