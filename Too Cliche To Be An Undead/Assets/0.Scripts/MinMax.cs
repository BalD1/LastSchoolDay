
using UnityEngine;

[System.Serializable]
public abstract class MinMax<T>
{
    [System.Serializable]
    public struct Data
    {
        public T minValue;  
        public T maxValue;

        public Data(T _minValue, T _maxValue)
        {
            minValue = _minValue;
            maxValue = _maxValue;
        }
    }

    [SerializeField] protected Data data;
    public Data GetData => data;

    public T Min => data.minValue;
    public T Max => data.maxValue;

    public MinMax(T _minValue, T _maxValue)
    {
        data = new Data(_minValue, _maxValue);
    }

    public abstract T Random();
}

[System.Serializable]
public class IntMinMax : MinMax<int>
{
    public IntMinMax(int _minValue, int _maxValue) : base(_minValue, _maxValue)
    {
        data = new Data(_minValue, _maxValue);
    }

    public override int Random()
    {
        return UnityEngine.Random.Range(Min, Max);
    }
}

[System.Serializable]
public class FloatMinMax : MinMax<float>
{
    public FloatMinMax(float _minValue, float _maxValue) : base(_minValue, _maxValue)
    {
        data = new Data(_minValue, _maxValue);
    }

    public override float Random()
    {
        return UnityEngine.Random.Range(Min, Max);
    }
}