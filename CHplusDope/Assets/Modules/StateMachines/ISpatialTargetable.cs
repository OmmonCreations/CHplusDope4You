using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace StateMachines
{
    public interface ISpatialTargetable : ITargetable
    {
        float Height { get; }
    }
}