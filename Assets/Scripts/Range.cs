using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range<T> where T : System.IConvertible
{
    public T min;
    public T max;
    public T GetRandom()
    {
        return (T)((System.IConvertible)Random.Range(min.ToSingle(null), max.ToSingle(null))).ToType(min.GetType(), null);
    }

    public Range(T min, T max)
    {
        this.min = min;
        this.max = max;
    }
}

[System.Serializable]
public class FloatRange : Range<float> {
    public FloatRange(float min, float max) : base(min, max) { }
    public FloatRange() : base(0, 0) { }

    public float Length => min - max;

    public float ReverseLerp(float value)
    {
        return Mathf.Clamp01((value - min) / Length);
    }
}

[System.Serializable]
public class IntRange : Range<int>
{
    public IntRange(int min, int max) : base(min, max) { }
    public IntRange() : base(0, 0) { }
}