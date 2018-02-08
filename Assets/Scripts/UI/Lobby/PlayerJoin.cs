using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XInputDotNetPure;

public class PlayerJoin : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform penContainer;
    [SerializeField] private GameObject penPrefab;
    [SerializeField] private GameObject startPrompt;

    [Header("Settings")]
    [SerializeField] private float penSpacing = 8;

    private PlayerPen[] playerPens = new PlayerPen[4];
    private bool[] joined = { false, false, false, false };
    private int nextSlotNumber;
    private List<int> ignoreIndices = new List<int>();

    void Start()
    {
        startPrompt.SetActive(false);

        // Set up pens
        for (int i = 0; i < 4; i++)
        {
            GameObject newPen = Instantiate(penPrefab, penContainer);
            newPen.transform.localPosition = Vector3.right * i * penSpacing;
            playerPens[i] = newPen.GetComponent<PlayerPen>();
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
        playerPens[nextSlotNumber].Init(index, ignoreIndices);
        nextSlotNumber++;
        UpdateHoverPosition();

        //TODO: 4 players auto start??
        if (nextSlotNumber == 2)
            startPrompt.SetActive(true);
    }

    private void UpdateHoverPosition()
    {
        if (nextSlotNumber < 4)
            playerPens[nextSlotNumber].SetAsNext();
    }

    private void StartGame()
    {
        Debug.Log("Start Game");
        Persistent.SetPlayerSlots(playerPens.Take(nextSlotNumber).Select(s => s.GetInfo()).ToList());
        SceneControl.ToLevelSelect();
    }
}
