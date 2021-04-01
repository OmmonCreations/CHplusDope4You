using System.Collections.Generic;
using DopeElections.Answer;
using DopeElections.PoliticalCharacters;
using UnityEngine;

namespace DopeElections.Candidates
{
    public class CandidateController : PoliticalCharacterController
    {
        private static readonly Dictionary<string, Material> MaterialChache = new Dictionary<string, Material>();

        [SerializeField] private Candidate _candidateValue = null;
        [SerializeField] private Sprite _defaultTexture = null;

        private Texture2D _portraitTexture;

        public Candidate Candidate
        {
            get => _candidateValue;
            set => ApplyCandidate(value);
        }

        public Texture2D Portrait
        {
            get => _portraitTexture;
            set => ApplyPortrait(value);
        }

        private void ApplyCandidate(Candidate candidate)
        {
            _candidateValue = candidate;
            /*if (_candidateValue.responses.Length != 0)
            {
                Debug.Log("candidate "+_candidateValue.firstName+" response : " +_candidateValue.responses[0]);
            }*/

            UpdateColorAndPicture();
        }

        private void UpdateColorAndPicture()
        {
            var candidate = _candidateValue;

            BodyColor = candidate.GetPartyColor();

            _candidateValue.GetIcon(_defaultTexture).Assign(s =>
            {
                if (!s) return;

                Portrait = s.texture;
            });
        }

        private void ApplyPortrait(Texture2D portrait)
        {
            _portraitTexture = portrait;
           
            var id = portrait.name;
            var materials = BodyRenderer.sharedMaterials;
            Material faceMaterial;
            if (MaterialChache.TryGetValue(id, out var existing))
            {
                faceMaterial = existing;
            }
            else
            {
                faceMaterial = Instantiate(materials[2]);
                
                var width = portrait.width;
                var height = portrait.height;

                // var minDim = Mathf.Min(width, height);
                var aspectRatio = (float) width / height;
                var scaleX = 1 / Mathf.Max(1, aspectRatio);
                var scaleY = 1 * Mathf.Min(1, aspectRatio);

                var scale = new Vector2(scaleX, scaleY);

                var offsetX = (1 - scaleX) / 2;
                var offsetY = (1 - scaleY) / 2;
                var offset = new Vector2(offsetX, offsetY);

                faceMaterial.mainTexture = portrait;
                faceMaterial.mainTextureOffset = offset;
                faceMaterial.mainTextureScale = scale;
                
                MaterialChache.Add(id, faceMaterial);
            }

            materials[2] = faceMaterial;
            BodyRenderer.sharedMaterials = materials;
        }
    }
}