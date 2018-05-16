using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private AudioClip beepSound;
    [SerializeField] private Light warnLight;
    [SerializeField] private Image radial;

    private Light mainLight;
    private int timeRemaining;
    private Color cameraOriginal;
    private float mainIntensity;
    private int roundTime = Persistent.Configs.time;

    public void Init()
    {
        timeRemaining = roundTime;
        cameraOriginal = Camera.main.backgroundColor;
        mainLight = GameObject.FindGameObjectWithTag("MainLight").GetComponent<Light>();
        mainIntensity = mainLight.intensity;
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        while (timeRemaining > 0)
        {
            text.text = timeRemaining.ToString();
            radial.fillAmount = 1 - (float)timeRemaining / roundTime;
            yield return new WaitForSeconds(1);
            timeRemaining--;
            if (timeRemaining == 11)
                StartCoroutine(FlashRed());
            if (0 <= timeRemaining && timeRemaining <= 10)
                SoundManager.PlayClip(beepSound, 1f / (timeRemaining + 3));
        }
        yield return new WaitForSeconds(0.5f);
        warnLight.intensity = 0;
        Camera.main.backgroundColor = cameraOriginal;
        mainLight.intensity = mainIntensity;
        SceneControl.ToEndGame();
    }

    private IEnumerator FlashRed()
    {
        yield return new WaitForSeconds(0.5f);
        float flashTime = 0.5f;
        while (true)
        {
            // Sine colour lerping
            float lerpValue = (Mathf.Cos(flashTime * Mathf.PI * 2) + 1) / 2f;
            lerpValue = lerpValue * lerpValue;
            warnLight.intensity = lerpValue * 0.4f;
            Camera.main.backgroundColor = Color.Lerp(cameraOriginal, Color.red, lerpValue);

            // Dimming main light
            mainLight.intensity = mainIntensity * (1 - ((flashTime - 0.5f) / 20f));

            flashTime += Time.deltaTime;
            yield return null;
        }
    }
}
