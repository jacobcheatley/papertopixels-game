using UnityEngine;
using XInputDotNetPure;

public class PlayerJoin : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private RectTransform emptySlotHover;

    private PlayerSlot[] slots = new PlayerSlot[4];
    private bool[] joined = { false, false, false, false };
    private int currentSlot;
    private GamePadState[] prevStates = new GamePadState[4];

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            // Create slots
            GameObject slot = Instantiate(slotPrefab, transform);
            RectTransform rt = slot.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(i / 3f, 0.5f);
            slot.SetActive(false);
            slots[i] = slot.GetComponent<PlayerSlot>();
        }

        currentSlot = 0;
        UpdateHoverPosition();
    }

    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            PlayerIndex index = (PlayerIndex) i;
            GamePadState state = GamePad.GetState(index);

            if (state.Buttons.A == ButtonState.Pressed && !joined[i])
            {
                joined[i] = true;
                AssignNextSlot(index);
            }
            else if (state.Buttons.Start == ButtonState.Pressed && currentSlot > 1)
                StartGame();
            else if (joined[i] && Controller.LeftPress(prevStates[i], state))
                slots[i].PrevColor();
            else if (joined[i] && Controller.RightPress(prevStates[i], state))
                slots[i].NextColor();

            prevStates[i] = state;
        }
    }

    void AssignNextSlot(PlayerIndex index)
    {
        slots[currentSlot].Init(index);
        currentSlot++;
        UpdateHoverPosition();

        //TODO: 4 players auto start??
    }

    void UpdateHoverPosition()
    {
        emptySlotHover.anchorMin = emptySlotHover.anchorMax = slots[currentSlot].GetComponent<RectTransform>().anchorMin;
    }

    void StartGame()
    {
        Debug.Log("Start Game");
    }
}
