using System.Text;
using System;

// almost costless abstraction over setting items in array, adds some safety 
// and meaningful error messages as well as pretty printing through toString method
public class Observations
{
    float[] array;
    int insertionPointer;
    string nextKey;
    int propertyCounter;

    StringBuilder stringBuilder = new StringBuilder("", 256);
    string[] properties;
    int[] propertyLengths;

    public Observations(string[] properties, int expectedCells)
    {
        this.properties = properties;
        propertyLengths = new int[properties.Length];
        array = new float[expectedCells];
        nextKey = properties[0];
    }

    void SetCheck(string key)
    {
        if (propertyCounter == properties.Length)
            throw new InvalidOperationException($"All propertes have already been set, attempted to set {key}.");
        if (key != properties[propertyCounter])
            throw new InvalidOperationException($"Expected key {nextKey} according to properties array, got {key}.");
        if (insertionPointer >= array.Length)
            throw new InvalidOperationException($"Too many values have been written. One of the arrays must've been too big, current key {key}.");
    }

    public void Set(string key, float[] value)
    {
        SetCheck(key);
        propertyLengths[propertyCounter] = value.Length;
        value.CopyTo(array, insertionPointer);
        insertionPointer += value.Length;
        propertyCounter++;
    }

    public void Set(string key, float value)
    {
        SetCheck(key);
        propertyLengths[propertyCounter] = 1;
        array[insertionPointer] = value;
        insertionPointer++;
        propertyCounter++;
    }

    public void SetZeros(string key, int cellsCount)
    {
        SetCheck(key);
        propertyLengths[propertyCounter] = cellsCount;
        insertionPointer += cellsCount;
        propertyCounter++;
    }

    public float[] ToArray()
    {
        return array;
    }

    public string toString()
    {
        StringBuilder stringBuilder = new StringBuilder(256);
        int propertyIndex = 0;
        int currentCell = 0;
        foreach (string property in properties)
        {
            stringBuilder.Append($"{property} : ");
            bool first = true;
            int end = currentCell + propertyLengths[propertyIndex];
            for (; currentCell < end; currentCell++)
            {
                if (!first)
                    stringBuilder.Append(", ");
                stringBuilder.Append(array[currentCell].ToString("0.##"));
                first = false;
            }
            stringBuilder.Append("\n");
            propertyIndex++;
        }

        return stringBuilder.ToString();
    }

    public void EndSetting()
    {
        if (propertyCounter != properties.Length)
            throw new InvalidOperationException($"Not all properties have been set, expected {properties[propertyCounter]}.");
        if (insertionPointer != array.Length)
            throw new InvalidOperationException("Not enough values have been written. One of the arrays must've been too small.");
    }
}