using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class CurrentlySelecting : MonoBehaviour
{
    [SerializeField] private Image playerIndicator;
    [SerializeField] private Image background;
    [SerializeField] private Text playerNumberText;

    private static CurrentlySelecting instance;

    void Start()
    {
        instance = this;
    }

    // Yes, this is gross
    public static void Init()
    {
        PlayerIndex index = Persistent.LobbyController;
        instance.SetIndex(index);
    }

    private void SetIndex(PlayerIndex index)
    {
        playerIndicator.rectTransform.rotation = UIHelper.PlayerIndicatorRotation(index);
        int playerNumber = Persistent.PlayerSlots.FindIndex(si => si.Index == index);
        background.color = Persistent.PlayerSlots[playerNumber].Color;
        playerNumberText.text = $"Player {playerNumber + 1}\nSelecting";
    }
}
