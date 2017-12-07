using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class LevelLoader : MonoBehaviour
{
    [Header("Test")]
    [SerializeField] private int id = 0;

    [Header("Function")]
    [SerializeField] private Transform levelContainer;
    [SerializeField] private float allScale = 2;

    [Header("Visual")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Material groundMaterial;

    private bool loading;
    private Map map;


    void Start()
    {
        StartCoroutine(Load());
    }

    private IEnumerator Load()
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
        GenerateGround();
        GenerateMarkers();
    }

    #region Generation
    private void GenerateGround()
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
            GameObject wall = new GameObject("Wall");
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
    #endregion
}
