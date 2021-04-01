using Procedures;

namespace DopeElections.Startup
{
    public abstract class DopeElectionsStartupStep : ProcedureStep<bool>
    {
        protected DopeElectionsApp App { get; }
        
        protected DopeElectionsStartupStep(DopeElectionsApp app)
        {
            App = app;
        }
    }
}