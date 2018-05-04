using System.Collections;
using UnityEngine;

public class KillFairy : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailerRenderer;
    [SerializeField] private Light spotLight;
    [SerializeField] private Rigidbody rb;

    private Transform target;
    private Vector3 initialVelocity;
    private float speed = 15;
    private float elapsedTime = 0;
    private float timeToDirect = 1.5f;

    public void Init(Color color, Transform target)
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        initialVelocity = new Vector3(dir.x, 0, dir.y) / 2f;

        trailerRenderer.startColor = color;
        trailerRenderer.endColor = new Color(color.r, color.g, color.b, 0);
        spotLight.color = color;

        this.target = target;
        StartCoroutine(Travel());
    }

    private IEnumerator Travel()
    {
        while (true)
        {
            Vector3 diff = (target.position - transform.position).normalized;
            rb.velocity = Vector3.Lerp(initialVelocity, diff, elapsedTime / timeToDirect) * speed;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == target)
        {
            Debug.Log("COLLECTED");
            Destroy(gameObject);
        }
    }
}
