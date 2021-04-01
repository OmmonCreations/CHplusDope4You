using System;
using System.Linq;
using DopeElections.Placeholders;
using DopeElections.PoliticalCharacters;
using DopeElections.SmartSpiders;
using Essentials;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Users
{
    public class PlayerController : PoliticalCharacterController
    {
        [SerializeField] private MiniSmartSpider _smartSpider = null;
        [SerializeField] private Image _faceImge = null;
        [SerializeField] private FaceType[] _faceTypes = null;
        [SerializeField] private AnimationCurve _jumpCurve = AnimationCurve.Constant(0, 1, 0);

        private NamespacedKey _face;

        public NamespacedKey Face
        {
            get => _face;
            set => ApplyFace(value);
        }

        public JumpToPositionState JumpTo(Vector3 localPosition, Quaternion localRotation, Vector3 localScale,
            float height,
            float time = 0.5f)
        {
            var state = new JumpToPositionState(this, localPosition, localRotation, localScale, height, time,
                _jumpCurve);
            StateMachine.State = state;
            return state;
        }

        public void UpdateSmartSpider()
        {
            _smartSpider.Value = DopeElectionsApp.Instance.User.SmartSpider.Values;
        }

        public void ApplyUserConfiguration(ActiveUser user = null)
        {
            if (user == null) user = DopeElectionsApp.Instance.User;
            var faceSelected = user.FaceId != default;
            Face = faceSelected ? user.FaceId : PlayerFaceId.Missing;

            UpdateSmartSpider();
        }

        private void ApplyFace(NamespacedKey id)
        {
            var type = _faceTypes.FirstOrDefault(t => t.Id == id);
            if (type == null)
            {
                return;
            }

            _face = id;
            _faceImge.sprite = type.Sprite;
        }

        [Serializable]
        public class FaceType
        {
            [SerializeField] private string _id = null;
            [SerializeField] private Sprite _sprite = null;

            public NamespacedKey Id => NamespacedKey.TryParse(_id, out var id) ? id : default;
            public Sprite Sprite => _sprite;
        }
    }
}