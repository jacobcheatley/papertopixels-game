using System.Linq;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] private MeshFilter filter;
    [SerializeField] private MeshCollider col;

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

        filter.mesh = mesh;
        col.sharedMesh = mesh;
        transform.position = Vector3.up * 0.5f;
    }

    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    void OnTriggerStay(Collider other)
    {
        HandleCollision(other);
    }

    private void HandleCollision(Collider other)
    {
        if (other.CompareTag("Player"))
            other.gameObject.GetComponentInParent<Player>().LavaHit();
    }
}
