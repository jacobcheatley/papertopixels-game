using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : LevelLoaderBase
{
    [SerializeField] private float spawnResolution = 1f;
    [SerializeField] private LayerMask noSpawnMask;
    private static List<Vector3> validSpawns;

    protected override void GenerateLevel()
    {
        base.GenerateLevel();
        DetermineSpawns();
        PlacePlayers();
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

            GameObject player = Instantiate(prefabs.playerPrefab, validSpawns[bestIndex], Quaternion.identity, levelContainer);
            player.GetComponent<Player>().Init(Persistent.PlayerSlots[p]);
            Persistent.PlayerObjects.Add(player);
        }
    }

    public static List<Vector3> ValidSpawns()
    {
        return validSpawns;
    }
}
