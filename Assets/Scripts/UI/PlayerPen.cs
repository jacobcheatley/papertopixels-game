using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerPen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject joinText;
    [SerializeField] private GameObject playerIndicator;
    [SerializeField] private MeshRenderer[] borderRenderers;
    [SerializeField] private GameObject playerPrefab;

    [Header("Setings")]
    [SerializeField] private Color[] colors;

    private PlayerIndex playerIndex;
    private int colorIndex;
    private int slotIndex;
    private List<int> ignoreIndices;
    private GamePadState state;
    private GamePadState prevState;
    private Material wallMaterial;
    private Player player;

    void Awake()
    {
        // Hide UI
        playerIndicator.SetActive(false);
        joinText.SetActive(false);

        // Set up wall materials
        wallMaterial = new Material(borderRenderers[0].material);
        foreach (MeshRenderer meshRenderer in borderRenderers)
            meshRenderer.material = wallMaterial;
    }

    public void Init(PlayerIndex index, List<int> ignoreIndices)
    {
        this.ignoreIndices = ignoreIndices;

        // Set up player index
        playerIndex = index;
        playerIndicator.SetActive(true);
        playerIndicator.transform.localRotation = UIHelper.PlayerIndicatorRotation(index);

        // Create player
        GameObject playerObject = Instantiate(playerPrefab, transform.position, Quaternion.identity, transform);
        player = playerObject.GetComponent<Player>();
        player.Init(GetInfo());

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

        joinText.SetActive(false);
    }

    public void SetAsNext()
    {
        joinText.SetActive(true);
    }

    void Update()
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);

        if (state.DPad.Left == ButtonState.Pressed && prevState.DPad.Left == ButtonState.Released)
            PrevColor();
        else if (state.DPad.Right == ButtonState.Pressed && prevState.DPad.Right == ButtonState.Released)
            NextColor();
    }

    private void DisplayColor()
    {
        ignoreIndices[slotIndex] = colorIndex;
        wallMaterial.color = colors[colorIndex];
        player.SetColor(colors[colorIndex]);
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