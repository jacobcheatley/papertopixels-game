using System.Linq;
using UnityEngine;
using XInputDotNetPure;

public class PlayerJoin : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private RectTransform emptySlotHover;
    [SerializeField] private GameObject startPrompt;

    private PlayerSlot[] slots = new PlayerSlot[4];
    private bool[] joined = { false, false, false, false };
    private int nextSlotNumber;

    void Start()
    {
        startPrompt.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            // Create slots
            GameObject slot = Instantiate(slotPrefab, transform);
            RectTransform rt = slot.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(i / 3f, 0.5f);
            slot.SetActive(false);
            slots[i] = slot.GetComponent<PlayerSlot>();
        }

        nextSlotNumber = 0;
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
            else if (state.Buttons.Start == ButtonState.Pressed && nextSlotNumber > 1)
            {
                StartGame();
                break;
            }
        }
    }

    private void AssignNextSlot(PlayerIndex index)
    {
        slots[nextSlotNumber].Init(index);
        nextSlotNumber++;
        UpdateHoverPosition();

        //TODO: 4 players auto start??
        if (nextSlotNumber == 2)
            startPrompt.SetActive(true);
    }

    private void UpdateHoverPosition()
    {
        emptySlotHover.anchorMin = emptySlotHover.anchorMax = slots[nextSlotNumber].GetComponent<RectTransform>().anchorMin;
    }

    private void StartGame()
    {
        Debug.Log("Start Game");
        Persistent.SetPlayerSlots(slots.Take(nextSlotNumber).Select(s => s.GetInfo()).ToList());
        SceneControl.ToLevelSelect();
    }
}
