using VolatileHordes.ActiveDirectors;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordeDirector
    {
        private readonly ActiveDirector _director;
        private readonly WanderingHordeSettings _settings;
        private readonly GamestageCalculator _gamestageCalculator;
        private readonly WanderingHordeCalculator _hordeCalculator;
        private readonly SpawningPositions _spawningPositions;
        private readonly WanderingHordeSpawner _spawner;

        public WanderingHordeDirector(
            ActiveDirector director,
            WanderingHordeSettings settings,
            GamestageCalculator gamestageCalculator,
            WanderingHordeCalculator hordeCalculator,
            SpawningPositions spawningPositions,
            WanderingHordeSpawner spawner)
        {
            _director = director;
            _settings = settings;
            _gamestageCalculator = gamestageCalculator;
            _hordeCalculator = hordeCalculator;
            _spawningPositions = spawningPositions;
            _spawner = spawner;
        }

        public void Spawn(int? size = null)
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;

            int noHorde = 0;
            size ??= _hordeCalculator.GetHordeSize(_settings, ref noHorde,
                _gamestageCalculator.GetEffectiveGamestage());
            
            Logger.Info("Spawning horde of size {0} at {1}", size, spawnTarget);

            var group = new ZombieGroup();
            _spawner.SpawnHorde(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.TriggerOrigin, size.Value, group);
            _director.TrackGroup(group);
        }
    }
}