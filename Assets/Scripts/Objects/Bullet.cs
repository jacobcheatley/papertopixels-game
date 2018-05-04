using UnityEngine;
using XInputDotNetPure;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TrailRenderer trailRenderer;

    [Header("Gameplay")]
    [SerializeField] private float explosionForce;
    [SerializeField] private float explosionRadius;

    // Serializing Fields because wew Unity is annoying
    [HideInInspector] [SerializeField] private Color color;
    [HideInInspector] [SerializeField] private PlayerIndex index;

    private bool exploding = false;

    public void Init(SlotInfo slotInfo)
    {
        index = slotInfo.Index;

        // Appearance
        SetColor(slotInfo.Color);
    }

    public void SetColor(Color color)
    {
        this.color = color;

        Material newMaterial = new Material(meshRenderer.material) {color = color};
        meshRenderer.material = newMaterial;

        // Trail renderer color
        trailRenderer.startColor = color;
        trailRenderer.endColor = new Color(color.r, color.g, color.b, 0);
    }

    void OnCollisionEnter(Collision other)
    {
        GameObject g = other.gameObject;

        if (g.tag == "Bullet")
        {
            Explode();
        }
        else if (g.tag == "Player")
        {
            Persistent.PlayerStats[index].ShotsHit++;
            Player player = g.GetComponent<Player>();
            Explode();
            if (player.Damage(index))
                Persistent.PlayerStats[index].ShotKills++;
        }
        else if (g.tag == "Wall")
        {
            Explode();
        }
    }

    void Explode()
    {
        if (exploding) return;
        exploding = true;

        SoundManager.PlayExplodeSound();

        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        // TODO: Smarter way of changing color?
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = explosion.GetComponent<ParticleSystem>().colorOverLifetime;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(color, 0), },
            new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1) }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(grad);
        Destroy(explosion, 2f);

        Vector3 pos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(pos, explosionRadius);

        foreach (Collider hit in colliders)
        {
            // TODO: Check there's line of sight?
            if (hit.CompareTag("Bullet") && hit.gameObject != gameObject)
                hit.GetComponent<Bullet>().Explode();
            else
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddExplosionForce(explosionForce, pos, explosionRadius, 0.01f, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }
}
