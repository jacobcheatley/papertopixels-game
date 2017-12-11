using UnityEngine;
using XInputDotNetPure;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TrailRenderer trailRenderer;

    [HideInInspector] public PlayerIndex playerIndex;
    [HideInInspector] public Color color;
    private Collider col;

    public void Init(SlotInfo slotInfo)
    {
        playerIndex = slotInfo.Index;

        // Appearance
        color = slotInfo.Color;
        Material newMaterial = new Material(meshRenderer.material);
        newMaterial.color = color;
        meshRenderer.material = newMaterial;

        // Trail renderer color
        trailRenderer.startColor = color;
        trailRenderer.endColor = new Color(color.r, color.g, color.b, 0);
    }

    void Start()
    {
        col = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision other)
    {
        GameObject g = other.gameObject;

//        if (g.tag == "Player" && g.GetComponent<Player>().playerIndex == playerIndex)
//        {
//            Physics.IgnoreCollision(other.col, col);
//            Debug.Log("Ignored");
//        }

        if (g.tag == "Bullet" && g.GetComponent<Bullet>().playerIndex == playerIndex)
        {
            Physics.IgnoreCollision(other.collider, col);
            return;
        }

        if (g.tag == "Wall")
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            // TODO: Smarter way of changing color?
            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = explosion.GetComponent<ParticleSystem>().colorOverLifetime;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new[] {new GradientColorKey(color, 0),},
                new[] {new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1)}
                );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(grad);
            Destroy(explosion, 2f);
            Destroy(gameObject);
            return;
        }
    }
}
