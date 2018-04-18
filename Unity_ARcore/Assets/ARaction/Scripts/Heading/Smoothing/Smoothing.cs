using UnityEngine;

namespace ARaction
{
    public abstract class Smoothing : ScriptableObject
    {
        public float LastSmoothed { get; private set; }

        public float GetSmoothed(float f)
        {
            LastSmoothed = CalcLastSmoothed(f);
            return LastSmoothed;
        }
        protected abstract float CalcLastSmoothed(float f);

        public abstract void Reset();
    }
}