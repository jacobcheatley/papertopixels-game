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

    public void Init(PlayerIndex index)
    {
        // Set up player index
        playerIndex = index;

        colorImage = GetComponent<Image>();

        switch (index)
        {
            case PlayerIndex.One:
                playerIndicator.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case PlayerIndex.Two:
                playerIndicator.transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            case PlayerIndex.Three:
                playerIndicator.transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case PlayerIndex.Four:
                playerIndicator.transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
        }

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

    public Color GetColor()
    {
        return colors[colorIndex];
    }
}
