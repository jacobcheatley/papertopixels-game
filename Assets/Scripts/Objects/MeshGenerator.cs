using System.Linq;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField] private MeshFilter filter;
    [SerializeField] private MeshCollider col;
    [SerializeField] private float height = 0.05f;
    [SerializeField] private float textureScale = 10f;

    public void Init(Vector3[] points)
    {
        Vector2[] points2d = points.Select(v => new Vector2(v.x, v.z)).ToArray();

        Triangulator tr = new Triangulator(points2d);
        int[] indices = tr.Triangulate();

        Mesh mesh = new Mesh();
        mesh.vertices = points;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.uv = points2d.Select(v => v / textureScale).ToArray();
        mesh.RecalculateTangents();

        filter.mesh = mesh;
        col.sharedMesh = mesh;
        transform.position = Vector3.up * height;
    }
}
