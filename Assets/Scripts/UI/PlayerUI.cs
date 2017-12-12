using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image dashCooldownImage;
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
            transform.position = toFollow.transform.position + offset;
    }

    public void StartCooldown(float time)
    {
        StartCoroutine(CooldownDisplay(time));
    }

    private IEnumerator CooldownDisplay(float time)
    {
        dashCooldownImage.gameObject.SetActive(true);

        float startTime = Time.time;
        float endTime = startTime + time;

        while (Time.time < endTime)
        {
            dashCooldownImage.fillAmount = (Time.time - startTime) / time;
//            dashCooldownImage.fillAmount = 1f;
            yield return null;
        }

        dashCooldownImage.gameObject.SetActive(false);
    }
}
