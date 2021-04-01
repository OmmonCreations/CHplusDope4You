using System.Linq;
using Essentials;
using UnityEngine;

namespace Progression
{
    public class ProgressionTree
    {
        public IProgressEntry[] Entries { get; }

        public ProgressionTree(params IProgressEntry[] entries)
        {
            Entries = entries;
        }

        public IProgressEntry GetEntry(NamespacedKey id)
        {
            return Entries.FirstOrDefault(e => e.Id == id);
        }

        public T GetEntry<T>(NamespacedKey id) where T : IProgressEntry
        {
            return Entries.OfType<T>().FirstOrDefault(e => e.Id == id);
        }

        public void Load(ProgressionData data)
        {
            var entries = Entries;
            foreach (var entrySection in data)
            {
                var entryId = entrySection.Key;
                var entryData = entrySection.Value;
                var entry = entries.FirstOrDefault(e => e.Id == entryId);
                if (entry == null)
                {
                    Debug.LogWarning($"Tried loading progress data for non existent entry id:{entryId}:\n{entryData}");
                    continue;
                }

                entry.Load(entryData);
            }
        }

        public ProgressionData Save()
        {
            var result = new ProgressionData();
            foreach (var entry in Entries)
            {
                var entryData = entry.Save();
                if (entryData == null) continue;
                result[entry.Id] = entryData;
            }

            return result;
        }
    }
}