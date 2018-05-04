using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Text playerText;
    [Header("Values")]
    [SerializeField] private Text killsText;
    [SerializeField] private Text deathsText;
    [SerializeField] private Text damageText;
    [SerializeField] private Text accuracyText;
    [SerializeField] private Text dashesText;

    private string[] placingStrings = {"1st", "2nd", "3rd", "4th"};

    public void Init(SlotInfo slotInfo, Vector2 position, int playerNumber)
    {
        GameStats stats = Persistent.PlayerStats[slotInfo.Index];
        rectTransform.localPosition = position;

        // Set up texts
        playerText.text = $"Player {playerNumber}";
        killsText.text = stats.TotalKills.ToString();
        deathsText.text = stats.Deaths.ToString();
        damageText.text = $"{stats.DamageDealt} : {stats.DamageTaken}";
        accuracyText.text = $"{stats.ShotsHit} / {stats.ShotsFired}";
        dashesText.text = stats.DashesLaunched.ToString();

        // Set up image components
        backgroundImage.color = slotInfo.Color;
        indicatorImage.rectTransform.rotation = UIHelper.PlayerIndicatorRotation(slotInfo.Index);

        // Placing
        int placing = Persistent.PlayerPlacings().IndexOf(slotInfo.Index);
        medalText.text = placingStrings[placing];
        medalImage.color = medalColors[placing];
    }
}
