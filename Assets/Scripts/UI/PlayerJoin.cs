using System.Collections;
using System.Collections.Generic;
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
    private List<int> ignoreIndices = new List<int>();

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
            else if (state.Buttons.Start == ButtonState.Pressed && nextSlotNumber > 1 && joined[i])
            {
                Persistent.LobbyController = index;
                StartGame();
                break;
            }
        }
    }

    private void AssignNextSlot(PlayerIndex index)
    {
        StartCoroutine(VibrateJoin(index));
        slots[nextSlotNumber].Init(index, ignoreIndices);
        nextSlotNumber++;
        if (nextSlotNumber == 4)
            emptySlotHover.gameObject.SetActive(false);
        else
            UpdateHoverPosition();

        //TODO: 4 players auto start??
        if (nextSlotNumber == 2)
            startPrompt.SetActive(true);
    }

    private IEnumerator VibrateJoin(PlayerIndex index)
    {
        GamePad.SetVibration(index, 0.3f, 0.3f);
        yield return new WaitForSeconds(0.1f);
        GamePad.SetVibration(index, 0, 0);
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
