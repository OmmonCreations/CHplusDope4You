using UnityEngine;

namespace DopeElections.Races.RaceTracks
{
    public class RaceTrackPartInstance
    {
        public RaceTrackPartController Controller { get; }
        public float Position { get; }

        public float Length => Controller.Length;
        public float EndPosition => Position + Length;
        public bool Visible => Controller && Controller.gameObject.activeSelf;

        public RaceTrackPartInstance(RaceTrackPartController controller, float position)
        {
            Controller = controller;
            Position = position;
        }

        public void Show() => Show(true);
        public void Hide() => Show(false);

        public void Show(bool show)
        {
            if (show == Visible) return;
            Controller.gameObject.SetActive(show);
        }
    }
}