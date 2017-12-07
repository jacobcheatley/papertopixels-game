using System;
using System.Linq;
using UnityEngine;

public class Map
{
    public int ID;
    public float Ratio;
    public Line[] Lines;

    public float HScale;
    public float VScale;
    public Vector3 Offset;
    public float Resolution;

    public Map(string json, float allScale)
    {
        // JSON load
        JSONMap temp = JsonUtility.FromJson<JSONMap>(json);

        // Set up variables
        ID = temp.id;
        Ratio = temp.ratio;
        Resolution = temp.resolution;

        if (Ratio >= 1f)
        {
            HScale = Ratio * allScale;
            VScale = allScale;
        }
        else
        {
            HScale = allScale;
            VScale = allScale / Ratio;
        }

        Offset = new Vector3(-HScale / 2, 0, VScale / 2);

        Lines = temp.lines.Select(l => new Line(l, this)).ToArray();
    }

    public override string ToString()
    {
        return $"Map #{ID} - Ratio: {Ratio}\n{string.Join("\n", Lines.Select(l => l.ToString()))}";
    }
}

public enum MapColor
{
    Black,
    Red,
    Green,
    Blue
}

public static class MapColorExtensions
{
    public static Color GetColor(this MapColor color)
    {
        switch (color)
        {
            case MapColor.Black:
                return Color.black;
            case MapColor.Red:
                return Color.red;
            case MapColor.Green:
                return Color.green;
            case MapColor.Blue:
                return Color.blue;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
    }
}

public class Line
{
    public Vector3[] Points;
    public MapColor Color;
    public bool Closed;

    public Line(JSONLine jsonLine, Map map)
    {
        Points = new Vector3[jsonLine.points.Length];
        float xMult = map.HScale / map.Resolution;
        float yMult = -map.VScale / map.Resolution;

        for (int i = 0; i < jsonLine.points.Length; i++)
            Points[i] = new Vector3(jsonLine.points[i].x * xMult, 0, jsonLine.points[i].y * yMult) + map.Offset;

        switch (jsonLine.color)
        {
            case "r":
                Color = MapColor.Red;
                break;
            case "g":
                Color = MapColor.Green;
                break;
            case "b":
                Color = MapColor.Blue;
                break;
            case "k":
                Color = MapColor.Black;
                break;
            default:
                Color = MapColor.Black;
                break;
        }

        Closed = jsonLine.closed;
    }

    public override string ToString()
    {
        return $"{(Closed ? "Closed" : "Open")} {Color} {string.Join(",", Points.Select(p => $"({p.x:##.00} {p.z:##.00})"))}";
    }
}