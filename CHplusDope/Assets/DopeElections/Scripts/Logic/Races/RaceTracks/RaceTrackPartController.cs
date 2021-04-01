using System.Linq;
using CameraSystems;
using UnityEngine;

namespace DopeElections.Races.RaceTracks
{
    public class RaceTrackPartController : MonoBehaviour
    {
        [SerializeField] private float _length = 1;
        [Header("Runtime")] [SerializeField] private float _position = 0;

        public float Position
        {
            get => _position;
            set => _position = value;
        }

        public float Length => _length;

        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}