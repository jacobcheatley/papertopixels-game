using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    [SerializeField] private MeshRenderer bodyRenderer;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Light spotlight;

    private Color color;

    public void SetColor(Color color)
    {
        this.color = color;

        // Body renderer material color
        Material newMaterial = new Material(bodyRenderer.material);
        newMaterial.color = color;
        bodyRenderer.material = newMaterial;

        // Trail renderer color
        trailRenderer.startColor = color;
        trailRenderer.endColor = new Color(color.r, color.g, color.b, 0);

        spotlight.color = color;
    }

    public Color GetColor()
    {
        return color;
    }
}
