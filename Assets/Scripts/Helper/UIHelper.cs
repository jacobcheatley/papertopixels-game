using UnityEngine;
using XInputDotNetPure;

public static class UIHelper
{
    public static Quaternion PlayerIndicatorRotation(PlayerIndex index)
    {
        switch (index)
        {
            case PlayerIndex.One:
                return Quaternion.Euler(0, 0, 0);
            case PlayerIndex.Two:
                return Quaternion.Euler(0, 0, 270);
            case PlayerIndex.Three:
                return Quaternion.Euler(0, 0, 90);
            case PlayerIndex.Four:
                return Quaternion.Euler(0, 0, 180);
            default:
                return Quaternion.Euler(0, 0, 0); // Shouldn't ever happen
        }
    }
}
