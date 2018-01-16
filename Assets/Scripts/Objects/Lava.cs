using System.Linq;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] private MeshFilter filter;
    [SerializeField] private MeshCollider col;

    public void Init(Vector3[] points)
    {
        Triangulator tr = new Triangulator(points.Select(v => new Vector2(v.x, v.z)).ToArray());
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
