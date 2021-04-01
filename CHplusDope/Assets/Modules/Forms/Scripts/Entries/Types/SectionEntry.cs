namespace Forms.Types
{
    public class SectionEntry : FormEntry
    {
        public FormEntry[] Entries { get; }

        public SectionEntry(FormEntry[] entries)
        {
            Entries = entries;
        }
    }
}