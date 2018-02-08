using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private RectTransform statCardContainer;
    [SerializeField] private GameObject statCardPrefab;
    [SerializeField] private TimerUI timerUi;

    private static InGameUI instance;

    void Start()
    {
        instance = this;
    }

    public static void Init()
    {
        int count = 0;
        foreach (SlotInfo slotInfo in Persistent.PlayerSlots)
        {
            Instantiate(instance.statCardPrefab, instance.statCardContainer).GetComponent<StatCard>().Init(slotInfo, count);
            count++;
        }
        instance.timerUi.Init();
    }
}
