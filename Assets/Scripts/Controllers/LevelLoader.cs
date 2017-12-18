using System.Collections;
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

    [Header("Function")]
    [SerializeField] private Transform levelContainer;
    [SerializeField] private float allScale = 2;
    [SerializeField] private float wallThickness = 0.2f;

    [Header("Visual")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Material groundMaterial;

    private bool loading;
    private Map map;
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
        Debug.Log(map);
        PlaceGround();
//        GenerateMarkers();
        GenerateWalls();
        PlaceAmmo();

        PlacePlayers();
    }

    #region Generation
    private void PlaceGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.parent = levelContainer;
        ground.transform.localScale = new Vector3(map.HScale / 10, 1, map.VScale / 10);
        ground.GetComponent<MeshRenderer>().material = groundMaterial;
        ground.name = "Ground";
    }

    private void GenerateMarkers()
    {
        foreach (Line line in map.Lines)
        {
            GameObject wall = new GameObject("Marker");
            wall.transform.parent = levelContainer;

            LineRenderer lRend = wall.AddComponent<LineRenderer>();
            lRend.positionCount = line.Points.Length;
            lRend.SetPositions(line.Points.Select(p => p + Vector3.up * 0.5f).ToArray());
            lRend.startWidth = lRend.endWidth = 0.25f;
            lRend.loop = line.Closed;
            lRend.shadowCastingMode = ShadowCastingMode.Off;
            lRend.receiveShadows = false;
            lRend.material = lineMaterial;
            lRend.startColor = line.Color.GetColor();
            lRend.endColor = line.Color.GetColor();
        }
    }

    private void GenerateWalls()
    {
        foreach (Line line in map.Lines.Where(l => l.Color == MapColor.Black))
        {
            GameObject wall = new GameObject();
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

    private void PlaceAmmo()
    {
        foreach (Line line in map.Lines.Where(l => l.Color == MapColor.Blue))
        {
            Vector3 avg = line.Average();
            float radius = line.Points.Average(p => Vector3.Distance(p, avg));

            GameObject ammo = Instantiate(ammoPrefab, avg, Quaternion.identity, levelContainer);
            ammo.transform.localScale = new Vector3(radius * 2, 1, radius * 2);
        }
    }

    private void PlacePlayers()
    {
        foreach (SlotInfo playerSlot in Persistent.PlayerSlots)
        {
            GameObject player = Instantiate(playerPrefab, Vector3.up, Quaternion.identity, levelContainer);
            player.GetComponent<Player>().Init(playerSlot);
        }
    }
    #endregion

    public static Transform LevelContainer()
    {
        return instance.levelContainer;
    }
}
