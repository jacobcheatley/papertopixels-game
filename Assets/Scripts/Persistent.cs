using System.Collections.Generic;
using UnityEngine;

public class Persistent : MonoBehaviour
{
    public static List<SlotInfo> PlayerSlots;

    public static void SetPlayerSlots(List<SlotInfo> slotsJoining)
    {
        PlayerSlots = slotsJoining;
    }
}
