using System;
using UnityEngine;
using XInputDotNetPure;

public class PlayerSlot : MonoBehaviour
{
    [SerializeField] private GameObject playerIndicator;

    private PlayerIndex index;

    public void Init(PlayerIndex index)
    {
        this.index = index;

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

        gameObject.SetActive(true);
    }

    // TODO: Cycle through colour, start game (?) when everyone's ready
    // Maps flow - WTF do?
}
