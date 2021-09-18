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
                default:
                    throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }
        }
    }
}