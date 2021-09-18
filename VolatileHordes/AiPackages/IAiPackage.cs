namespace VolatileHordes.AiPackages
{
    public interface IAiPackage
    {
        AiPackageEnum TypeEnum { get; }

        void ApplyTo(ZombieGroup group);
    }
}