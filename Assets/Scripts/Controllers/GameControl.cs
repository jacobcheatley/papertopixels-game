using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    [SerializeField] private GameObject respawnObject;
    [SerializeField] private float respawnTime;

    private static GameControl instance;
    private List<Vector3> locations;

    void Start()
    {
        instance = this;
    }

    public static void Respawn(Player player)
    {
        instance.StartCoroutine(instance.RespawnProcess(player));
    }

    IEnumerator RespawnProcess(Player player)
    {
        player.gameObject.SetActive(false);

        Vector3 spawnLocation = Vector3.up; // TODO: Set location dynamically

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
