using UnityEngine;

public class Lava : MeshGenerator
{
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
