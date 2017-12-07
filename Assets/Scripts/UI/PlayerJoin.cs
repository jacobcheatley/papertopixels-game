using UnityEngine;
using XInputDotNetPure;

public class PlayerJoin : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;

    private PlayerSlot[] slots = new PlayerSlot[4];
    private bool[] joined = { false, false, false, false };
    private int currentSlot;

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject slot = Instantiate(slotPrefab, transform);
            RectTransform rt = slot.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(i / 3f, 0.5f);
            slot.SetActive(false);
            slots[i] = slot.GetComponent<PlayerSlot>();
        }

        currentSlot = 0;
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
        }
    }

    void AssignNextSlot(PlayerIndex index)
    {
        slots[currentSlot].Init(index);
        currentSlot++;
    }
}
