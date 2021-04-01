using UnityEngine;

namespace StateMachines
{
    public interface ITargetable
    {
        Vector3 Position { get; }
        Quaternion Rotation { get; }
    }
}