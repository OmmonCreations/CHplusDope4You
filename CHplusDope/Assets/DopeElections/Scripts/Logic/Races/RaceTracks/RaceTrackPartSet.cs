using CameraSystems;
using Essentials;
using UnityEngine;

namespace DopeElections.Races.RaceTracks
{
    [CreateAssetMenu(fileName = "RaceTrackPartSet", menuName = "Dope Elections/Race Track Part Set")]
    public class RaceTrackPartSet : ScriptableObject
    {
        [SerializeField] private float _width = 9;
        [SerializeField] private float _viewLength = 50;
        [SerializeField] private int _candidateLod = 1;
        [SerializeField] private CameraTransformation _cameraTransformation = default;
        [SerializeField] private RaceTrackPartController[] _parts = null;

        public float Width => _width;
        public float ViewLength => _viewLength;
        public int CandidateLod => _candidateLod;
        public CameraTransformation CameraTransformation => _cameraTransformation;
        public RaceTrackPartController[] Parts => _parts;
    }
}