using HarmonyLib;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Hooks
{
    [HarmonyPatch(typeof(World))]
    [HarmonyPatch("SpawnEntityInWorld")]
    class AddZombieSpawnTracking
    {
        static bool Prefix(Entity _entity)
        {
            if (_entity is EntityZombie zombie)
            {
                Container.Ambient.ZombieSpawned(zombie.entityId);
            }
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