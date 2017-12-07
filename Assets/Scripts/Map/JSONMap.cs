using System;

[Serializable]
public class JSONMap
{
    public int id;
    public float ratio;
    public int resolution;
    public JSONLine[] lines;
}

[Serializable]
public class JSONLine
{
    public string color;
    public bool closed;
    public JSONPoint[] points;
}

[Serializable]
public class JSONPoint
{
    public int x;
    public int y;
}