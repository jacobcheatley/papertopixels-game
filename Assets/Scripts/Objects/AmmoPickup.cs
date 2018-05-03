class AmmoPickup : Pickup
{
    protected override bool PickupEffect(Player player)
    {
        if (!player.RefillAmmo()) return false;

        SoundManager.PlayReloadSound();
        return true;
    }
}
