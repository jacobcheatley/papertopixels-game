using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private int roundTime = 90;
    [SerializeField] private Text text;
    [SerializeField] private AudioClip beepSound;

    private int timeRemaining;

    public void Init()
    {
        timeRemaining = roundTime;
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        while (timeRemaining > 0)
        {
            text.text = timeRemaining.ToString();
            yield return new WaitForSeconds(1);
            timeRemaining--;
            if (0 < timeRemaining && timeRemaining <= 10)
                SoundManager.PlayClip(beepSound, 1f / timeRemaining);
        }
        SceneControl.ToEndGame();
    }
}
