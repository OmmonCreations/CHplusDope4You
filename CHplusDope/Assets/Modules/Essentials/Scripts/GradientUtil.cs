using UnityEngine;

namespace Essentials
{
    public static class GradientUtil
    {
        public static Gradient Create(params Color[] colors)
        {
            var keys = new GradientColorKey[colors.Length];
            var step = 1f / colors.Length; 
            
            for (var i = 0; i < colors.Length; i++)
            {
                keys[i] = new GradientColorKey(colors[i], step * i);
            }

            return new Gradient() {colorKeys = keys, mode = GradientMode.Blend};
        }
    }
}