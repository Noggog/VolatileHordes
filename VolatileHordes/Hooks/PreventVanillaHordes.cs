using HarmonyLib;

namespace VolatileHordes.Hooks
{
    [HarmonyPatch(typeof(AIDirectorWanderingHordeComponent))]
    [HarmonyPatch("SpawnWanderingHorde")]
    class PreventVanillaHordes
    {
        static bool Prefix(bool feral)
        {
            // Prevent hordes from spawning.
            return false;
        }
    }
}