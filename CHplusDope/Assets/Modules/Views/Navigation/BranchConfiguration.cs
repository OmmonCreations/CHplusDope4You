namespace Views
{
    public struct BranchConfiguration
    {
        public static BranchConfiguration Default{get{return new BranchConfiguration();}}
        
        public bool ShowOnAwake { get; set; }
    }
}