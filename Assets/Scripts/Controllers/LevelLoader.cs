﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class LevelLoader : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject wallCorner;
    [SerializeField] private GameObject wallSegment;
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private GameObject healthPrefab;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject lavaPrefab;

    [Header("Function")]
    [SerializeField] private Transform levelContainer;
    [SerializeField] private float allScale = 2;
    [SerializeField] private float wallThickness = 0.2f;
    [SerializeField] private float spawnResolution = 1f;
    [SerializeField] private LayerMask noSpawnMask;

    [Header("Visual")]
    [SerializeField] private Material lineMaterial;

    [Header("UI")]
    [SerializeField] private InGameUI gameUI;

    private bool loading;
    private Map map;
    private List<Vector3> validSpawns;
    private static LevelLoader instance;

    void Start()
    {
        instance = this;
    }

    public static void Create(int id)
    {
        instance.StartCoroutine(instance.Load(id));
    }

    private IEnumerator Load(int id)
    {
        if (!loading)
        {
            loading = true;
            string mapText = null;

            if ((mapText = FileCache.LoadMap(id)) == null)
            {
                using (UnityWebRequest www = UnityWebRequest.Get($"http://papertopixels.tk/map/{id}"))
                {
                    yield return www.SendWebRequest();

                    if (www.isNetworkError || www.isHttpError)
                        Debug.Log(www.error);
                    else
                    {
                        mapText = www.downloadHandler.text;
                        FileCache.SaveMap(mapText, id);
                    }
                }
            }

            if (mapText != null)
            {
                map = new Map(mapText, allScale);
                GenerateLevel();
            }

            loading = false;
        }
        yield return null;
    }

    private void GenerateLevel()
    {
//        GenerateMarkers();
        PlaceGround();
        GenerateWalls();
        GenerateLava();
        PlaceAmmo();
        PlaceHealth();

        DetermineSpawns();
        PlacePlayers();
    }

    #region Generation
    private void PlaceGround()
    {
        Instantiate(groundPrefab, levelContainer);
        // TODO: ATM everything is hard coded to be 2*sqrt(2) ratio
    }

    private void GenerateMarkers()
    {
        foreach (Line line in map.Lines)
            MakeLineRenderer(line, line.Color.GetColor());
    }

    private void MakeLineRenderer(Line line, Color color, Transform parent = null)
    {
        if (parent == null) parent = levelContainer;

        GameObject wall = new GameObject("Line Renderer");
        wall.transform.parent = parent;

        LineRenderer lRend = wall.AddComponent<LineRenderer>();
        lRend.positionCount = line.Points.Length;
        lRend.SetPositions(line.Points.Select(p => p + Vector3.up * 0.5f).ToArray());
        lRend.startWidth = lRend.endWidth = 0.25f;
        lRend.loop = line.Closed;
        lRend.shadowCastingMode = ShadowCastingMode.Off;
        lRend.receiveShadows = false;
        lRend.material = lineMaterial;
        lRend.startColor = color;
        lRend.endColor = color;
    }

    private void GenerateWalls()
    {
        foreach (Line line in map.Lines.Where(l => l.Color == MapColor.Black))
        {
            GameObject wall = new GameObject("Wall");
            wall.transform.parent = levelContainer;
            for (int i = 0; i < line.Points.Length - 1; i++)
            {
                Vector3 point = line.Points[i];
                Vector3 next = line.Points[i + 1];

                // Create corner
                if (i != 0)
                    Instantiate(wallCorner, point + Vector3.up, Quaternion.identity, wall.transform);

                // Create wall segment
                GameObject section = Instantiate(wallSegment, (point + next) / 2f + Vector3.up, Quaternion.LookRotation(point - next), wall.transform);
                section.transform.localScale = new Vector3(wallThickness, 2, Vector3.Distance(point, next));
            }

            if (line.Closed)
            {
                // Create last little segment for closed loops
                Vector3 point = line.Points[line.Points.Length - 1];
                Vector3 next = line.Points[0];

                // Create corners
                Instantiate(wallCorner, point + Vector3.up, Quaternion.identity, wall.transform);
                Instantiate(wallCorner, next + Vector3.up, Quaternion.identity, wall.transform);

                // Create wall segment
                GameObject section = Instantiate(wallSegment, (point + next) / 2f + Vector3.up, Quaternion.LookRotation(point - next), wall.transform);
                section.transform.localScale = new Vector3(wallThickness, 2, Vector3.Distance(point, next));
            }
        }
    }

    private void GenerateLava()
    {
        foreach (Line line in map.Lines.Where(l => l.Color == MapColor.Red && l.Closed))
        {
            Lava lava = Instantiate(lavaPrefab, levelContainer).GetComponent<Lava>();
            lava.Init(line.Points);
            MakeLineRenderer(line, new Color(0.85f, 0.05f, 0), lava.transform);
        }
    }

    private void PlaceAmmo()
    {
        foreach (Line line in map.Lines.Where(l => l.Color == MapColor.Blue && l.Closed))
        {
            Vector3 avg = line.Average();
            float radius = line.Points.Average(p => Vector3.Distance(p, avg));

            GameObject ammo = Instantiate(ammoPrefab, avg, Quaternion.identity, levelContainer);
            ammo.transform.localScale = new Vector3(radius * 2, 1, radius * 2);
        }
    }

    private void PlaceHealth()
    {
        foreach (Line line in map.Lines.Where(l => l.Color == MapColor.Green && l.Closed))
        {
            Vector3 avg = line.Average();
            float radius = line.Points.Average(p => Vector3.Distance(p, avg));

            GameObject health = Instantiate(healthPrefab, avg, Quaternion.identity, levelContainer);
            health.transform.localScale = new Vector3(radius * 2, 1, radius * 2);
        }
    }

    private void DetermineSpawns()
    {
        float xDist = map.HScale / 2;
        float zDist = map.VScale / 2;

        validSpawns = new List<Vector3>();

        for (float zz = -zDist; zz <= zDist; zz += spawnResolution)
        {
            for (float xx = -xDist; xx <= xDist; xx += spawnResolution)
            {
                Vector3 location = new Vector3(xx, 1, zz);
                Collider[] walls = Physics.OverlapSphere(location, spawnResolution * 1.2f, noSpawnMask);
                if (walls.Length == 0)
                    validSpawns.Add(location);
            }
        }
    }

    private void PlacePlayers()
    {
        // Vectors for distance comparison
        Vector3[] comparisons = {
            new Vector3(-map.HScale / 2, 0, map.VScale / 2), // TOP LEFT
            new Vector3(map.HScale / 2, 0, -map.VScale / 2), // BOTTOM RIGHT
            new Vector3(-map.HScale / 2, 0, -map.VScale / 2), // BOTTOM LEFT
            new Vector3(map.HScale / 2, 0, map.VScale / 2) // TOP RIGHT
        };

        for (int p = 0; p < Persistent.PlayerSlots.Count; p++)
        {
            float bestDistance = 100;
            int bestIndex = 0;

            for (var i = 0; i < validSpawns.Count; i++)
            {
                Vector3 spawn = validSpawns[i];

                float distance = Vector3.Distance(comparisons[p], spawn);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = i;
                }
            }

            GameObject player = Instantiate(playerPrefab, validSpawns[bestIndex], Quaternion.identity, levelContainer);
            player.GetComponent<Player>().Init(Persistent.PlayerSlots[p]);
            Persistent.PlayerObjects.Add(player);
        }
    }
    #endregion

    public static Transform LevelContainer()
    {
        return instance.levelContainer;
    }

    public static List<Vector3> ValidSpawns()
    {
        return instance.validSpawns;
    }
}
