using Essentials;
using Newtonsoft.Json.Linq;

namespace Progression
{
    public interface IProgressEntry
    {
        NamespacedKey Id { get; }
        string State { get; set; }

        void Load(JToken data);
        JToken Save();
    }
}