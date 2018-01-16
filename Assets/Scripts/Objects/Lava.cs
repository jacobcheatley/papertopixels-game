using System.Linq;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] private MeshFilter filter;
    [SerializeField] private MeshCollider col;

    public void Init(Vector3[] points)
    {
        Vector2[] points2d = points.Select(v => new Vector2(v.x, v.z)).ToArray();
        // Ensure clockwise
        if (!Triangulator.PolygonCW(points2d))
        {
            Debug.Log(points2d[0]);
            Debug.Log("werp");
            points2d = points2d.Reverse().ToArray();
            points = points.Reverse().ToArray();
            Debug.Log(points2d[0]);
        }

        Triangulator tr = new Triangulator(points2d);
        int[] indices = tr.Triangulate();

        Mesh mesh = new Mesh();
        mesh.vertices = points;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        filter.mesh = mesh;
        col.sharedMesh = mesh;
        transform.position = Vector3.up * 0.5f;
    }
}
