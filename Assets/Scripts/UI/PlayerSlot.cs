using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerSlot : MonoBehaviour
{
    [SerializeField] private GameObject playerIndicator;
    [SerializeField] private Color[] colors;
    [SerializeField] private Image colorImage;

    private PlayerIndex playerIndex;
    private int colorIndex;
    private int slotIndex;
    private static List<int> ignoreIndices = new List<int>();
    private GamePadState state;
    private GamePadState prevState;

    public void Init(PlayerIndex index)
    {
        // Set up player index
        playerIndex = index;

        colorImage = GetComponent<Image>();

        playerIndicator.transform.rotation = UIHelper.PlayerIndicatorRotation(index);

        // Set up colors
        slotIndex = ignoreIndices.Count;

        for (int i = 0; i < colors.Length; i++)
        {
            if (!ignoreIndices.Contains(i))
            {
                colorIndex = i;
                ignoreIndices.Add(i);
                DisplayColor();
                break;
            }
        }

        gameObject.SetActive(true);
    }

    void Update()
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);
        
        if (Controller.LeftPress(prevState, state))
            PrevColor();
        else if (Controller.RightPress(prevState, state))
            NextColor();
    }

    private void DisplayColor()
    {
        colorImage.color = colors[colorIndex];
        ignoreIndices[slotIndex] = colorIndex;
    }

    public void NextColor()
    {
        int indexToTry = colorIndex;

        while (ignoreIndices.Contains(indexToTry))
        {
            indexToTry = (indexToTry + 1) % colors.Length;
        }

        colorIndex = indexToTry;
        DisplayColor();
    }

    public void PrevColor()
    {
        int indexToTry = colorIndex;

        while (ignoreIndices.Contains(indexToTry))
        {
            indexToTry--;
            if (indexToTry < 0) indexToTry = colors.Length - 1;
        }

        colorIndex = indexToTry;
        DisplayColor();
    }

    public SlotInfo GetInfo()
    {
        return new SlotInfo
        {
            Color = colors[colorIndex],
            Index = playerIndex
        };
    }
}

public struct SlotInfo
{
    public PlayerIndex Index;
    public Color Color;

    public override string ToString()
    {
        return $"{Index} {Color}";
    }
}