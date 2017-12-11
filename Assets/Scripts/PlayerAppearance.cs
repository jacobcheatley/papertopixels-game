using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    [SerializeField] private MeshRenderer bodyRenderer;
    [SerializeField] private TrailRenderer trailRenderer;

    public void SetColor(Color color)
    {
        // Body renderer material color
        Material newMaterial = new Material(bodyRenderer.material);
        newMaterial.color = color;
        bodyRenderer.material = newMaterial;

        // Trail renderer color
        trailRenderer.startColor = color;
        trailRenderer.endColor = new Color(color.r, color.g, color.b, 0);
    }
}
