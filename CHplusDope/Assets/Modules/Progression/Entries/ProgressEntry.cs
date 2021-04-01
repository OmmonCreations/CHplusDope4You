using Essentials;
using Newtonsoft.Json.Linq;

namespace Progression
{
    public abstract class ProgressEntry : IProgressEntry
    {
        private string _state;

        public NamespacedKey Id { get; }

        public string State
        {
            get => _state;
            set => ApplyState(value);
        }

        public bool IsAvailable => State == ProgressState.Completed || State == ProgressState.Unlocked;

        protected ProgressEntry(NamespacedKey id)
        {
            Id = id;
            _state = ProgressState.Unknown;
        }

        private void ApplyState(string value)
        {
            _state = value;
            OnStateChanged(value);
        }

        protected virtual void OnStateChanged(string state)
        {
        }

        public void Load(JToken data)
        {
            if (!(data is JObject section)) return;
            if(section["state"] != null) State = (string) section["state"];
            OnLoad(section["data"]);
        }

        protected virtual void OnLoad(JToken data)
        {
        }

        public JToken Save()
        {
            var result = new JObject();
            if (State == ProgressState.Completed) result["state"] = State;
            var data = SaveData();
            if (data != null) result["data"] = data;
            return result.Count > 0 ? result : null;
        }

        protected virtual JToken SaveData()
        {
            return null;
        }

        public static class ProgressState
        {
            public const string Locked = "l";
            public const string Completed = "c";
            public const string Unlocked = "u";
            public const string Unknown = "?";
        }
    }
}