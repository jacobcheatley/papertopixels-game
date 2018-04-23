using UnityEngine;
using UnityEngine.UI;

public class Pulse : MonoBehaviour
{
    private Image image;
    [SerializeField] private float frequency = 2f;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        image.color = new Color(1, 1, 1, (Mathf.Sin(Time.time * frequency) + 1) / 2f);
    }
}
