using System.Collections.Generic;

namespace Forms.Types
{
    public interface ISelectOptionsProvider
    {
        List<SelectOption> GetOptions(string property);
    }
}