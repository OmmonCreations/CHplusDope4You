using Newtonsoft.Json.Linq;

namespace Progression
{
    public interface IProgressUserData
    {
        JToken Serialize();
    }
}