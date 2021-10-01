namespace VolatileHordes.NoiseViewer.Simulations
{
    public class SimulationContainer
    {
        public static readonly SimulationNoiseManager NoiseManager = new();
        public static readonly SimulationZombieControl ZombieControl = new();
        public static readonly SimulationLogger Logger = new();
        public static readonly Simulation Simulation = new();
    }
}