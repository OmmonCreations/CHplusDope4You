using Essentials;
using Progression;

namespace DopeElections.Progression
{
    public class CutsceneProgressEntry : ProgressEntry
    {
        public string CutsceneId { get; }
        
        public CutsceneProgressEntry(NamespacedKey id, string cutsceneId) : base(id)
        {
            CutsceneId = cutsceneId;
        }

    }
}