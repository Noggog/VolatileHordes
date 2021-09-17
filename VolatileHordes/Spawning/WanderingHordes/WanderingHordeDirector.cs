using System;
using System.Threading.Tasks;
using VolatileHordes.ActiveDirectors;
using VolatileHordes.ActiveDirectors.Roaming;
using VolatileHordes.Control;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordeDirector
    {
        private readonly ActiveDirector _director;
        private readonly WanderingHordeSettings _settings;
        private readonly RoamOccasionally _roamOccasionally;
        private readonly GamestageCalculator _gamestageCalculator;
        private readonly WanderingHordeCalculator _hordeCalculator;
        private readonly SpawningPositions _spawningPositions;
        private readonly WanderingHordeSpawner _spawner;
        private readonly ZombieControl _control;

        public WanderingHordeDirector(
            ActiveDirector director,
            WanderingHordeSettings settings,
            RoamOccasionally roamOccasionally,
            GamestageCalculator gamestageCalculator,
            WanderingHordeCalculator hordeCalculator,
            SpawningPositions spawningPositions,
            WanderingHordeSpawner spawner,
            ZombieControl control)
        {
            _director = director;
            _settings = settings;
            _roamOccasionally = roamOccasionally;
            _gamestageCalculator = gamestageCalculator;
            _hordeCalculator = hordeCalculator;
            _spawningPositions = spawningPositions;
            _spawner = spawner;
            _control = control;
        }

        public async Task Spawn(int? size = null)
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;

            int noHorde = 0;
            size ??= _hordeCalculator.GetHordeSize(_settings, ref noHorde,
                _gamestageCalculator.GetEffectiveGamestage());

            var group = _director.NewGroup();
            
            Logger.Info("Spawning horde {0} of size {1} at {2}", group.Id, size, spawnTarget);
            
            await _spawner.SpawnHorde(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.TriggerOrigin, size.Value, group);

            _control.SendGroupTowards(group, spawnTarget.TriggerOrigin);
            
            _roamOccasionally.ApplyTo(group);
        }
    }
}