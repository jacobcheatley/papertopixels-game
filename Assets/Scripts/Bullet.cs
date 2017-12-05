using UnityEngine;
using XInputDotNetPure;

public class Bullet : MonoBehaviour
{
    public PlayerIndex playerIndex;

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
            Debug.Log("Ignored");
        }

        Debug.Log(g.tag);
    }
}
