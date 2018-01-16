class HealthPickup : Pickup
{
    protected override void PickupEffect(Player player)
    {
        player.RefillHealth();
    }
}
