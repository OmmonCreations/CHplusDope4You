using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Forms.Types
{
    public interface ISelectOption
    {
        JToken Value { get; }
        string Label { get; }
        Sprite Sprite { get; }
    }
}