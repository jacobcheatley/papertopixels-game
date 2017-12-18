using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    [SerializeField] private float cooldown = 10;

    private bool stocked = true;

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
        if (other.tag == "Player" && stocked)
        {
            stocked = false;
            Player player = other.GetComponent<Player>();
            player.RefillAmmo();
            StartCoroutine(Restock());
        }
    }

    private IEnumerator Restock()
    {
        float percent = 0;

        while (percent < 1)
        {
            cooldownImage.fillAmount = 1 - percent;
            percent += Time.deltaTime / cooldown;
            yield return null;
        }

        cooldownImage.fillAmount = 0;
        stocked = true;
    }
}
