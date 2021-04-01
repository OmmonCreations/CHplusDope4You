using TMPro;
using UnityEngine;

namespace TextMesh_Pro_Curve
{
    [RequireComponent((typeof(TMP_Text)))]
    public class TextPro3dOnACircle : MonoBehaviour
    {
        private TMP_Text _mTextComponent;

        public AnimationCurve _vertexCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.25f, 2.0f),
            new Keyframe(0.5f, 0), new Keyframe(0.75f, 2.0f), new Keyframe(1, 0f));

        [SerializeField] private float _radius = 1.0f;

        private Vector3[] _vertices;
        private Matrix4x4 _matrix;

        private float _oldRadius;

        private void Awake()
        {
            _mTextComponent = gameObject.GetComponent<TMP_Text>();
        }


        private void Start()
        {
            _vertexCurve.preWrapMode = WrapMode.Clamp;
            _vertexCurve.postWrapMode = WrapMode.Clamp;

            //Mesh mesh = m_TextComponent.textInfo.meshInfo[0].mesh;

            _mTextComponent.havePropertiesChanged = true; // Need to force the TextMeshPro Object to be updated.

            _oldRadius = _radius;
        }

        private void LateUpdate()
        {
            if (!_mTextComponent.havePropertiesChanged && Mathf.Abs(_oldRadius - _radius) <= 0)
            {
                return;
            }

            _oldRadius = _radius;

            _mTextComponent
                .ForceMeshUpdate(); // Generate the mesh and populate the textInfo with data we can use and manipulate.

            TMP_TextInfo textInfo = _mTextComponent.textInfo;
            int characterCount = textInfo.characterCount;


            if (characterCount == 0) return;

            //vertices = textInfo.meshInfo[0].vertices;
            //int lastVertexIndex = textInfo.characterInfo[characterCount - 1].vertexIndex;

            float boundsMinX = _mTextComponent.bounds.min.x; //textInfo.meshInfo[0].mesh.bounds.min.x;
            float boundsMaxX = _mTextComponent.bounds.max.x; //textInfo.meshInfo[0].mesh.bounds.max.x;


            for (int i = 0; i < characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the index of the mesh used by this character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                _vertices = textInfo.meshInfo[materialIndex].vertices;

                // Compute the baseline mid point for each character
                Vector3 offsetToMidBaseline =
                    new Vector2((_vertices[vertexIndex + 0].x + _vertices[vertexIndex + 2].x) / 2,
                        textInfo.characterInfo[i].baseLine);
                //float offsetY = VertexCurve.Evaluate((float)i / characterCount + loopCount / 50f); // Random.Range(-0.25f, 0.25f);

                // Apply offset to adjust our pivot point.
                _vertices[vertexIndex + 0] += -offsetToMidBaseline;
                _vertices[vertexIndex + 1] += -offsetToMidBaseline;
                _vertices[vertexIndex + 2] += -offsetToMidBaseline;
                _vertices[vertexIndex + 3] += -offsetToMidBaseline;

                // Compute the angle of rotation for each character based on the animation curve
                float x0 = (offsetToMidBaseline.x - boundsMinX) /
                           (boundsMaxX - boundsMinX); // Character's position relative to the bounds of the mesh.
                float x1 = x0 + 0.0001f;
                float y0 = _vertexCurve.Evaluate(x0) * _radius;
                float y1 = _vertexCurve.Evaluate(x1) * _radius;

                Vector3 horizontal = new Vector3(1, 0, 0);
                //Vector3 normal = new Vector3(-(y1 - y0), (x1 * (boundsMaxX - boundsMinX) + boundsMinX) - offsetToMidBaseline.x, 0);
                Vector3 tangent = new Vector3(x1 * (boundsMaxX - boundsMinX) + boundsMinX, y1) -
                                  new Vector3(offsetToMidBaseline.x, y0);

                float dot = Mathf.Acos(Vector3.Dot(horizontal, tangent.normalized)) * 57.2957795f;
                Vector3 cross = Vector3.Cross(horizontal, tangent);
                float angle = cross.z > 0 ? dot : 360 - dot;

                _matrix = Matrix4x4.TRS(new Vector3(0, y0, 0), Quaternion.Euler(0, 0, angle), Vector3.one);

                _vertices[vertexIndex + 0] = _matrix.MultiplyPoint3x4(_vertices[vertexIndex + 0]);
                _vertices[vertexIndex + 1] = _matrix.MultiplyPoint3x4(_vertices[vertexIndex + 1]);
                _vertices[vertexIndex + 2] = _matrix.MultiplyPoint3x4(_vertices[vertexIndex + 2]);
                _vertices[vertexIndex + 3] = _matrix.MultiplyPoint3x4(_vertices[vertexIndex + 3]);

                _vertices[vertexIndex + 0] += offsetToMidBaseline;
                _vertices[vertexIndex + 1] += offsetToMidBaseline;
                _vertices[vertexIndex + 2] += offsetToMidBaseline;
                _vertices[vertexIndex + 3] += offsetToMidBaseline;
            }


            // Upload the mesh with the revised information
            _mTextComponent.UpdateVertexData();
        }

        private AnimationCurve CopyAnimationCurve(AnimationCurve curve)
        {
            AnimationCurve newCurve = new AnimationCurve();

            newCurve.keys = curve.keys;

            return newCurve;
        }
    }
}