using System.Collections;
using UnityEngine;

public class RespawnDisplay : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    public void Init(Color color, float time)
    {
        Material newMaterial = new Material(meshRenderer.material);
        newMaterial.color = color;
        meshRenderer.material = newMaterial;

        StartCoroutine(PulseColor(color, time));
    }

    private IEnumerator PulseColor(Color color, float time)
    {
        float percent = 0;

        while (percent < 1)
        {
            float alpha = (Mathf.Sin(2 * Mathf.PI * percent * percent * 8) + 1) / 2;
            meshRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
            percent += Time.deltaTime / time;
            yield return null;
        }
    }
}
