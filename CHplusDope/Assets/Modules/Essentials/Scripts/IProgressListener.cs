namespace Essentials
{
    public interface IProgressListener
    {
        void OnProgress(float progress);
        void OnFinish();
        void SetLabel(string label);
    }
}