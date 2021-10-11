using HarmonyLib;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Hooks
{
    [HarmonyPatch(typeof(AIDirector))]
    [HarmonyPatch("AddZombie")]
    class AddZombieSpawnTracking
    {
        static bool Prefix(EntityEnemy _zombie)
        {
            Container.Ambient.ZombieSpawned(_zombie.entityId);
            return true;
        }
    }
    
    [HarmonyPatch(typeof(AIDirector))]
    [HarmonyPatch("RemoveZombie")]
    class RemoveZombieSpawnTracking
    {
        static bool Prefix(EntityEnemy _zombie)
        {
            Container.Ambient.ZombieDespawned(_zombie.entityId);
            return true;
        }
    }
}