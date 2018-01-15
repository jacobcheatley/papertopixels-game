using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class EndStatCard : MonoBehaviour
{
    [Header("Design")]
    [SerializeField] private Color[] medalColors;
    [Header("References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image medalImage;
    [SerializeField] private Text medalText;
    [SerializeField] private Image indicatorImage;
    [Header("Values")]
    [SerializeField] private Text killsText;
    [SerializeField] private Text deathsText;
    [SerializeField] private Text damageText;

    private string[] placingStrings = { "1st", "2nd", "3rd", "4th" };

    public void Init(SlotInfo slotInfo, Vector2 position)
    {
        GameStats stats = Persistent.PlayerStats[slotInfo.Index];
        rectTransform.localPosition = position;

        // Set up texts
        killsText.text = stats.Kills.ToString();
        deathsText.text = stats.Deaths.ToString();
        damageText.text = $"{stats.DamageDealt} - {stats.DamageTaken}";

        // Set up image components
        backgroundImage.color = slotInfo.Color;
        indicatorImage.rectTransform.rotation = UIHelper.PlayerIndicatorRotation(slotInfo.Index);

        // TODO: Placings
    }
}
