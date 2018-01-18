using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private RectTransform statCardContainer;
    [SerializeField] private GameObject statCardPrefab;

    private static EndGameUI instance;
    private const int spacing = 320;

    void Start()
    {
        instance = this;
    }

    public static void Init()
    {
        int count = 0;
        foreach (SlotInfo slotInfo in Persistent.PlayerSlots)
        {
            Instantiate(instance.statCardPrefab, instance.statCardContainer).GetComponent<EndStatCard>().Init(slotInfo, new Vector2(count * spacing + 20, 0), count + 1);
            count++;
        }
        instance.statCardContainer.sizeDelta = new Vector2((count + 1) * (spacing / 2f), 100);
    }

    void Update()
    {
        for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
        {
            GamePadState state = GamePad.GetState(i);
            if (state.Buttons.Start == ButtonState.Pressed)
                SceneControl.Restart();
        }
    }
}
