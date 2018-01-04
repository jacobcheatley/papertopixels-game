using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    [SerializeField] private GameObject respawnObject;
    [SerializeField] private float respawnTime;

    private static GameControl instance;

    void Start()
    {
        instance = this;
    }

    public static void Respawn(Player player)
    {
        instance.StartCoroutine(instance.RespawnProcess(player));
    }

    private Vector3 GetBestSpawn(GameObject player)
    {
        List<Vector3> otherPlayerLocations = Persistent.PlayerObjects.Where(p => p != player).Select(p => p.transform.position).ToList();

        float greatestDistance = 0;
        Vector3 currentLocation = Vector3.up;

        foreach (Vector3 spawn in LevelLoader.ValidSpawns())
        {
            float minDistance = otherPlayerLocations.Min(loc => Vector3.Distance(loc, spawn));
            if (minDistance > greatestDistance)
            {
                greatestDistance = minDistance;
                currentLocation = spawn;
            }
        }

        return currentLocation;
    }

    IEnumerator RespawnProcess(Player player)
    {
        player.gameObject.SetActive(false);

        Vector3 spawnLocation = GetBestSpawn(player.gameObject); // TODO: Set location dynamically

        // Create respawn object
        Color color = player.GetColor();
        GameObject resp = Instantiate(respawnObject, spawnLocation, Quaternion.identity, LevelLoader.LevelContainer());
        RespawnDisplay robj = resp.GetComponent<RespawnDisplay>();
        robj.Init(color, respawnTime);

        player.transform.position = spawnLocation;
        yield return new WaitForSeconds(respawnTime);

        Destroy(resp);
        player.gameObject.SetActive(true);
    }
}
