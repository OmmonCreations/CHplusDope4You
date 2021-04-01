using UnityEngine;

namespace RandomUtils
{
    public static class Random
    {
        private static readonly System.Random Rand = new System.Random();
        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
        /// </summary>
        public static float Value => (float) Rand.NextDouble();
        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to min, and less than max.
        /// </summary>
        public static float Range(float min, float max) => min + (max - min) * Value;
        /// <summary>
        /// Returns a random integer number that is greater than or equal to min, and less than max.
        /// </summary>
        public static int Range(int min, int max) => min + Mathf.FloorToInt((max - min) * Value);
    }
}