using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image dashCooldownImage;
    [SerializeField] private Text ammoCountText;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Vector3 offset;

    private Transform toFollow;

    public void Init(Transform toFollow)
    {
        this.toFollow = toFollow;
    }

    void Start()
    {
        dashCooldownImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (toFollow != null)
        {
            transform.position = toFollow.transform.position + offset;
            canvas.SetActive(toFollow.gameObject.activeInHierarchy);
        }
    }

    public void SetHealth(int health, int maxHealth)
    {
        healthBarImage.fillAmount = (float)health / maxHealth;
    }

    public void SetAmmo(int newCount)
    {
        ammoCountText.text = newCount.ToString();
    }

    public void StartCooldown(float time)
    {
        StartCoroutine(CooldownDisplay(time));
    }

    private IEnumerator CooldownDisplay(float time)
    {
        dashCooldownImage.gameObject.SetActive(true);

        float percent = 0;

        while (percent < 1)
        {
            dashCooldownImage.fillAmount = percent;
            percent += Time.deltaTime / time;
            yield return null;
        }

        dashCooldownImage.gameObject.SetActive(false);
    }
}
