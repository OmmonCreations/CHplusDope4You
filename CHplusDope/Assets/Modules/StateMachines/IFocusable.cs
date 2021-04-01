namespace StateMachines
{
    public interface IFocusable : ISpatialTargetable
    {
        float PreferredFocusDistance { get; }
    }
}