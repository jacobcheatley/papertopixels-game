using System;
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
        var a = PlayerStats.ToList()
            .OrderByDescending(p => p.Value.Kills)
            .ThenBy(p => p.Value.Deaths)
            .ThenByDescending(p => p.Value.DamageDealt)
            .ThenBy(p => p.Value.DamageTaken)
            .Select(p => p.Key)
            .ToList();
        return a;
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