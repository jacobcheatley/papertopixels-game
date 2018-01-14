using UnityEngine;
using UnityEngine.UI;

public class StatCard : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image background;
    [SerializeField] private Image playerIndicator;
    [SerializeField] private Text killCounter;

    private GameStats stats;
    private int currentKills = 0;

    public void Init(SlotInfo slotInfo, int position)
    {
        // Top left, bottom right, bottom left, top right
        switch (position)
        {
            case 0:
                rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = Vector2.up;
                break;
            case 1:
                rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = Vector2.right;
                break;
            case 2:
                rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
                break;
            case 3:
                rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = Vector2.one;
                break;
            default:
                break;
        }

        playerIndicator.rectTransform.rotation = UIHelper.PlayerIndicatorRotation(slotInfo.Index);

        stats = Persistent.PlayerStats[slotInfo.Index];
        background.color = slotInfo.Color;
        killCounter.text = "<b>0</b>";
    }

    void Update()
    {
        if (stats.Kills != currentKills)
        {
            killCounter.text = $"<b>{stats.Kills}</b>";
        }
    }
}
