using System;
using System.IO;
using UnityEngine;

public static class FileCache
{
    private static string savePath =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/")}/My Games/PaperToPixels";

    public static Texture2D LoadThumb(int id)
    {
        string path = $"{savePath}/thumbs/{id}.png";

        if (!File.Exists(path))
            return null;

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(File.ReadAllBytes(path));

        return tex;
    }

    public static void SaveThumb(Texture2D texture, int id)
    {
        try
        {
            File.WriteAllBytes($"{savePath}/thumbs/{id}.png", texture.EncodeToPNG());
        }
        catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory($"{savePath}/thumbs");
            File.WriteAllBytes($"{savePath}/thumbs/{id}.png", texture.EncodeToPNG());
        }
    }

    public static string LoadMap(int id)
    {
        string path = $"{savePath}/maps/{id}.json";

        if (!File.Exists(path))
            return null;

        return File.ReadAllText(path);
    }

    public static void SaveMap(string json, int id)
    {
        try
        {
            File.WriteAllText($"{savePath}/maps/{id}.json", json);
        }
        catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory($"{savePath}/maps");
            File.WriteAllText($"{savePath}/maps/{id}.json", json);
        }
    }
}
