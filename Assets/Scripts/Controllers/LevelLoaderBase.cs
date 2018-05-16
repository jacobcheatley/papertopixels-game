using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class LevelLoaderBase : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] protected LevelPrefabStore prefabs;

    [Header("Function")]
    [SerializeField] protected Transform levelContainer;
    [SerializeField] private float allScale = 20;
    [SerializeField] private float wallThickness = 0.2f;

    [Header("Visual")]
    [SerializeField] private Material lineMaterial;

    private bool loading;
    protected Map map;
    protected static LevelLoaderBase instance;

    void Start()
    {
        instance = this;
    }
    
    public static void Create(int id)
    {
        instance.StartCoroutine(instance.Load(id));
    }

    public IEnumerator Load(int id) // public because access rights and MonoBehaviours are gross
    {
        if (!loading)
        {
            loading = true;
            string mapText = null;

            if ((mapText = FileCache.LoadMap(id)) == null)
            {
                using (UnityWebRequest www = UnityWebRequest.Get($"{Persistent.Configs.address}/map/{id}"))
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

    protected virtual void GenerateLevel()
    {
        PlaceGround();
        GenerateWalls();
        GenerateLava();
        PlaceAmmo();
        PlaceHealth();
    }

    #region Generation
    private void PlaceGround()
    {
        Instantiate(prefabs.groundPrefab, levelContainer);
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
        lRend.SetPositions(line.Points.Select(p => p + Vector3.up * 0.02f).ToArray());
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
                    Instantiate(prefabs.wallCorner, point + Vector3.up, Quaternion.identity, wall.transform);

                // Create wall segment
                GameObject section = Instantiate(prefabs.wallSegment, (point + next) / 2f + Vector3.up, Quaternion.LookRotation(point - next), wall.transform);
                section.transform.localScale = new Vector3(wallThickness, 2, Vector3.Distance(point, next));
            }

            if (line.Closed)
            {
                // Create last little segment for closed loops
                Vector3 point = line.Points[line.Points.Length - 1];
                Vector3 next = line.Points[0];

                // Create corners
                Instantiate(prefabs.wallCorner, point + Vector3.up, Quaternion.identity, wall.transform);
                Instantiate(prefabs.wallCorner, next + Vector3.up, Quaternion.identity, wall.transform);

                // Create wall segment
                GameObject section = Instantiate(prefabs.wallSegment, (point + next) / 2f + Vector3.up, Quaternion.LookRotation(point - next), wall.transform);
                section.transform.localScale = new Vector3(wallThickness, 2, Vector3.Distance(point, next));

                // Generate black central mesh
                MeshGenerator wallMesh = Instantiate(prefabs.wallDynamic, levelContainer).GetComponent<MeshGenerator>();
                wallMesh.Init(line.Points);
            }
        }
    }

    private void GenerateLava()
    {
        foreach (Line line in map.Lines.Where(l => l.Color == MapColor.Red && l.Closed))
        {
            Lava lava = Instantiate(prefabs.lavaPrefab, levelContainer).GetComponent<Lava>();
            lava.Init(line.Points);
            MakeLineRenderer(line, new Color(0.5f, 0.05f, 0, 0.65f), lava.transform);
        }
    }

    private void PlaceAmmo()
    {
        foreach (Line line in map.Lines.Where(l => l.Color == MapColor.Blue && l.Closed))
        {
            Vector3 avg = line.Average();
            float radius = line.Points.Average(p => Vector3.Distance(p, avg));

            GameObject ammo = Instantiate(prefabs.ammoPrefab, avg + Vector3.up * 0.75f, Quaternion.identity, levelContainer);
            ammo.transform.localScale = new Vector3(radius * 2, ammo.transform.localScale.y, radius * 2);
        }
    }

    private void PlaceHealth()
    {
        foreach (Line line in map.Lines.Where(l => l.Color == MapColor.Green && l.Closed))
        {
            Vector3 avg = line.Average();
            float radius = line.Points.Average(p => Vector3.Distance(p, avg));

            GameObject health = Instantiate(prefabs.healthPrefab, avg + Vector3.up * 0.75f, Quaternion.identity, levelContainer);
            health.transform.localScale = new Vector3(radius * 2, health.transform.localScale.y, radius * 2);
        }
    }
    #endregion

    public static Transform LevelContainer()
    {
        return instance.levelContainer;
    }
}