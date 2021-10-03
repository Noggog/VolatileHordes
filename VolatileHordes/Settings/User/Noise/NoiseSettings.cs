using System.Collections.Generic;

namespace VolatileHordes.Settings.User
{
    public class NoiseSettings
    {
        public List<NoiseItem> Noises { get; set; } = new()
        {
            new("ulmGunShootRifle01", 15),
            new("ulmPlasmaFire13", 15),
            new("ulmPlasmaFire07", 15),
            new("ulmLaserPistol_Fire", 15),
            new("44magnum_fire", 25),
            new("ulmPistolFire16", 15),
            new("ulmPistolFire14", 15),
            new("ulmPistolFire06", 15),
            new("ulmPistolFire01", 15),
            new("ulmPistolFire09", 15),
            new("ulmPistolFire12", 15),
            new("ulmMinigunFire", 15),
            new("ulmPistolFire11", 15),
            new("ulmGunShootShotgunBM4", 60),
            new("ulmMachineGunFire02", 15),
            new("ulmMachineGunFire015", 15),
            new("mp15_s_fire", 15),
            new("junkturret_fire", 5),
            new("m136_fire", 15),
            new("m60_fire", 15),
            new("tacticalar_fire", 15),
            new("ak47_fire", 15),
            new("sharpshooter_fire", 60),
            new("sniperrifle_fire", 60),
            new("autoshotgun_fire", 25),
            new("hunting_rifle_fire", 60),
            new("tacticalar_fire", 60),
            new("pump_shotgun_fire", 60),
            new("shotgundb_fire", 60),
            new("blunderbuss_fire", 70),
            new("desertvulture_fire", 15),
            new("mp15_fire", 15),
            new("Pistol_Fire", 15),
            new("Auger_Fire_Start", 3),
            new("chainsaw_fire_start", 3),
            new("explosion_grenade", 100),
            new("explosion_charge", 100),
            new("explosion3", 100),
            new("explosion2", 100),
            new("explosion1", 100),
            new("treefallimpact", 50),
        };
    }
}