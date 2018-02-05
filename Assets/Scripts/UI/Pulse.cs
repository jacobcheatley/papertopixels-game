using UnityEngine;
using UnityEngine.UI;

public class Pulse : MonoBehaviour
{
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        image.color = new Color(1, 1, 1, Mathf.Sin(Time.time));
    }
}
