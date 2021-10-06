using System;
using VolatileHordes.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.AiPackages
{
    public class AmbientAiPackage : IAiPackage
    {
        private readonly NoiseResponderControlFactory _noiseResponderControlFactory;
        public AiPackageEnum TypeEnum => AiPackageEnum.Ambient;

        public AmbientAiPackage(NoiseResponderControlFactory noiseResponderControlFactory)
        {
            _noiseResponderControlFactory = noiseResponderControlFactory;
        }
        
        public void ApplyTo(ZombieGroup group)
        {
            var noiseControl = _noiseResponderControlFactory.Create();
            noiseControl.ApplyTo(group)
                .Subscribe()
                .DisposeWith(group);
        }
    }
}