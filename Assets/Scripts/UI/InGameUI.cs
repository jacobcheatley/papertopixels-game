using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private RectTransform statCardContainer;
    [SerializeField] private GameObject statCardPrefab;
    [SerializeField] private TimerUI timerUi;

    public void Init()
    {
        int count = 0;
        foreach (SlotInfo slotInfo in Persistent.PlayerSlots)
        {
            Instantiate(statCardPrefab, statCardContainer).GetComponent<StatCard>().Init(slotInfo, count);
            count++;
        }
        timerUi.Init();
    }
}
