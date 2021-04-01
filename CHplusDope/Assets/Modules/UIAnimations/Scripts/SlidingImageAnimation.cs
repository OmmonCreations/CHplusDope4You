using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIAnimations
{
    public class SlidingImageAnimation : MonoBehaviour
    {
        [SerializeField] private RawImage _image = null;
        [SerializeField] private Vector2 _motion = new Vector2(1, 0);

        // Update is called once per frame
        void Update()
        {
            var rect = _image.uvRect;
            _image.uvRect = new Rect(rect.position + _motion * Time.deltaTime, rect.size);
        }
    }
}