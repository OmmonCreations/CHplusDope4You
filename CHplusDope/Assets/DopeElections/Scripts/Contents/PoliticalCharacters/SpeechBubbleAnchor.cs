using StateMachines;
using UnityEngine;

namespace DopeElections.PoliticalCharacters
{
    public class SpeechBubbleAnchor : MonoBehaviour, ISpatialTargetable
    {
        [SerializeField] private float _height = 2;
        
        public string Id => "speech_bubble_anchor";
        
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        public float Height => _height;
        public float PreferredFocusDistance => 10;
    }
}