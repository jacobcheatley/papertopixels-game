using System.Linq;
using UnityEngine;
using XInputDotNetPure;

public class PlayerJoin : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private RectTransform emptySlotHover;

    private PlayerSlot[] slots = new PlayerSlot[4];
    private bool[] joined = { false, false, false, false };
    private int currentSlot;

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
            {
                StartGame();
                break;
            }
        }
    }

    private void AssignNextSlot(PlayerIndex index)
    {
        slots[currentSlot].Init(index);
        currentSlot++;
        UpdateHoverPosition();

        //TODO: 4 players auto start??
    }

    private void UpdateHoverPosition()
    {
        emptySlotHover.anchorMin = emptySlotHover.anchorMax = slots[currentSlot].GetComponent<RectTransform>().anchorMin;
    }

    private void StartGame()
    {
        Debug.Log("Start Game");
        Persistent.SetPlayerSlots(slots.Take(currentSlot).Select(s => s.GetInfo()).ToList());
        SceneControl.ToLevelSelect();
    }
}
