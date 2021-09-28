using System;

namespace VolatileHordes.AiPackages
{
    public class AiPackageMapper
    {
        public IAiPackage? Get(AiPackageEnum? e)
        {
            if (e == null) return null;
            switch (e)
            {
                case AiPackageEnum.Roaming:
                    return Container.RoamAiPackage;
                case AiPackageEnum.Seeker:
                    return Container.SeekerAiPackage;
                case AiPackageEnum.Runner:
                    return Container.RunnerAiPackage;
                case AiPackageEnum.Crazy:
                    return Container.CrazyAiPackage;
                default:
                    throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }
        }
    }
}