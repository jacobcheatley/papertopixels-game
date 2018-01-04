using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Persistent : MonoBehaviour
{
    public static List<SlotInfo> PlayerSlots;
    public static Dictionary<PlayerIndex, GameStats> PlayerStats = new Dictionary<PlayerIndex, GameStats>();
    public static List<GameObject> PlayerObjects = new List<GameObject>();

    public static void SetPlayerSlots(List<SlotInfo> slotsJoining)
    {
        PlayerSlots = slotsJoining;
        foreach (SlotInfo playerSlot in PlayerSlots)
            PlayerStats.Add(playerSlot.Index, new GameStats());
    }
}

public class GameStats
{
    public int DamageDealt = 0;
    public int DamageTaken = 0;
    public int Kills = 0;
    public int Deaths = 0;
}