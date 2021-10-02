using System.Collections.Generic;
using System.Drawing;
using VolatileHordes.Control;
using VolatileHordes.Randomization;
using VolatileHordes.Settings.User.Control;
using VolatileHordes.Tracking;

namespace VolatileHordes.NoiseViewer.Simulations
{
    public class Simulation
    {
        private NoiseResponderControlFactory NoiseResponderControlFactory = new(
            new RandomSource(), SimulationContainer.ZombieControl,
            SimulationContainer.NoiseManager, new NoiseResponderSettings(),
            SimulationContainer.Logger);

        public List<SimulationZombieGroup> ZombieGroups = new();
        
        public void Reset()
        {
            CreateGroups();
        }

        private void CreateGroups()
        {
            ZombieGroups.Clear();
            AddGroupAt(new PointF(10, 10));
            AddGroupAt(new PointF(-20, 15));
            AddGroupAt(new PointF(30, -25));
            AddGroupAt(new PointF(-50, -35));
            AddGroupAt(new PointF(45, 30));
            AddGroupAt(new PointF(-65, 40));
            AddGroupAt(new PointF(75, -50));
            AddGroupAt(new PointF(5, -60));
            AddGroupAt(new PointF(-10, -80));
            AddGroupAt(new PointF(10, 90));
            AddGroupAt(new PointF(20, 55));
            AddGroupAt(new PointF(20, 110));
            AddGroupAt(new PointF(110, 30));
        }
        
        private void AddGroupAt(PointF pt)
        {
            ZombieGroups.Add(new SimulationZombieGroup(NoiseResponderControlFactory.Create())
            {
                Target = pt
            });
        }
    }
}