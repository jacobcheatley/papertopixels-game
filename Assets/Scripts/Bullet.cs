using UnityEngine;
using XInputDotNetPure;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    [HideInInspector] public PlayerIndex playerIndex;
    private Collider col;

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
            Destroy(explosion, 2f);
            Destroy(gameObject);
            return;
        }
    }
}
