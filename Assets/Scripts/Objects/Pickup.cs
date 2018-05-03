using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Pickup : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    [SerializeField] private float cooldown = 10;
    [SerializeField] private Light spotLight;

    private bool stocked = true;

    protected abstract bool PickupEffect(Player player);

    void Start()
    {
        Vector3 spotPos = spotLight.transform.position;
        spotLight.transform.position = new Vector3(spotPos.x, spotPos.y * transform.localScale.x, spotPos.z);
        StartCoroutine("Pulse");
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
        if (other.tag != "Player" || !stocked) return;
        Player player = other.GetComponent<Player>();
        if (!PickupEffect(player)) return;

        stocked = false;
        StartCoroutine(Restock());
    }

    private IEnumerator Pulse()
    {
        float minAngle = 30;
        float maxAngle = 60;
        float currentAngle = minAngle;
        float speed = 20;

        while (true)
        {
            while (currentAngle < maxAngle)
            {
                currentAngle += Time.deltaTime * speed;
                spotLight.spotAngle = currentAngle;
                yield return null;
            }
            while (currentAngle > minAngle)
            {
                currentAngle -= Time.deltaTime * speed;
                spotLight.spotAngle = currentAngle;
                yield return null;
            }
        }
    }

    private IEnumerator Restock()
    {
        float percent = 0;
        StopCoroutine("Pulse");
        spotLight.enabled = false;

        while (percent < 1)
        {
            cooldownImage.fillAmount = 1 - percent;
            percent += Time.deltaTime / cooldown;
            yield return null;
        }

        cooldownImage.fillAmount = 0;
        stocked = true;
        spotLight.enabled = true;
        StartCoroutine("Pulse");
    }
}
