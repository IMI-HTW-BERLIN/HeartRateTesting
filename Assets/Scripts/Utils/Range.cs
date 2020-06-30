using System;
using UnityEngine;

namespace Utils
{
    public class Range
    {
        public readonly float From;
        public readonly float To;

        public Range(float from, float to)
        {
            From = from;
            To = to;
        }

        public float GetInBetween(float unitInterval)
        {
            float interval = Mathf.Clamp(unitInterval, 0, 1);
            return (To - From) * interval + From;
        }
    }
}