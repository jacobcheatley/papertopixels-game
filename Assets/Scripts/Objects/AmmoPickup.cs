﻿class AmmoPickup : Pickup
{
    protected override void PickupEffect(Player player)
    {
        player.RefillAmmo();
    }
}
