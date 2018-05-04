class HealthPickup : Pickup
{
    protected override bool PickupEffect(Player player)
    {
        if (!player.RefillHealth()) return false;

        SoundManager.PlayHealSound();
        return true;
    }
}
