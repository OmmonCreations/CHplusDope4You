using System;

namespace DopeElections.Answer
{
    [Serializable]
    public class ScanResult
    {
        public string raw;
        public int[] candidates;
        public int[] lists;
    }
}