using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XInputDotNetPure;

public class Persistent : MonoBehaviour
{
    public static List<SlotInfo> PlayerSlots;
    public static Dictionary<PlayerIndex, GameStats> PlayerStats = new Dictionary<PlayerIndex, GameStats>();
    public static List<GameObject> PlayerObjects = new List<GameObject>();
    public static PlayerIndex LobbyController;

    public static void SetPlayerSlots(List<SlotInfo> slotsJoining)
    {
        PlayerSlots = slotsJoining;
        foreach (SlotInfo playerSlot in PlayerSlots)
            PlayerStats.Add(playerSlot.Index, new GameStats());
    }

    public static void Clear()
    {
        PlayerSlots.Clear();
        PlayerStats.Clear();
        PlayerObjects.Clear();
    }

    public static List<PlayerIndex> PlayerPlacings()
    {
        return PlayerStats.ToList()
            .OrderByDescending(p => p.Value.Kills) // Highest kills
            .ThenBy(p => p.Value.Deaths) // Lowest deaths
            .ThenByDescending(p => p.Value.DamageDealt) // Most damage dealt
            .ThenBy(p => p.Value.DamageTaken) // Least damage taken
            .ThenByDescending(p => (float)p.Value.ShotsHit / p.Value.ShotsFired) // Highest accuracy
            .Select(p => p.Key)
            .ToList();
    }
}

public class GameStats
{
    public int DamageDealt = 0;
    public int DamageTaken = 0;
    public int Kills = 0;
    public int Deaths = 0;
    public int ShotsFired = 0;
    public int ShotsHit = 0;
    public int Dashes = 0;
}