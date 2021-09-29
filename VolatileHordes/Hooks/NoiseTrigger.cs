using HarmonyLib;
using UnityEngine;

namespace VolatileHordes.Hooks
{
    [HarmonyPatch(typeof(AIDirector))]
    [HarmonyPatch("NotifyNoise")]
    class NoiseTrigger
    {
        static bool Prefix(Entity instigator, Vector3 position, string clipName, float volumeScale)
        {
            Logger.Verbose("Noise event \"{0}\", \"{1}\", \"{2}\", \"{3}\"", instigator, position, clipName, volumeScale);
            Container.NoiseManager.Notify(position, clipName, volumeScale);
            return true;
        }
    }
}