using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private int roundTime = 90;
    [SerializeField] private Text text;
    [SerializeField] private AudioClip beepSound;

    private int timeRemaining;
    private Light mainLight;

    public void Init()
    {
        timeRemaining = roundTime;
        mainLight = GameObject.FindGameObjectWithTag("MainLight").GetComponent<Light>();
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        while (timeRemaining > 0)
        {
            text.text = timeRemaining.ToString();
            yield return new WaitForSeconds(1);
            timeRemaining--;
            if (timeRemaining == 10)
                StartCoroutine(FlashRed());
            if (0 < timeRemaining && timeRemaining <= 10)
                SoundManager.PlayClip(beepSound, 1f / timeRemaining);
        }
        SceneControl.ToEndGame();
    }

    private IEnumerator FlashRed()
    {
        float flashTime = 0;
        while (true)
        {
            mainLight.color = Color.Lerp(Color.white, Color.red, (Mathf.Sin(flashTime * Mathf.PI * 2) + 1) / 2f); // Sine waves are fun
            flashTime += Time.deltaTime;
            yield return null;
        }
    }
}
