namespace MoreHordes
{
  public class Class1
    {

        bool CreateZombie(ZombieAgent zombie, PlayerZone zone)
        {
            if (!CanSpawnActiveZombie())
            {
                return false;
            }

            var world = GameManager.Instance.World;
            Chunk? chunk = world.GetChunkSync(World.toChunkXZ(Mathf.FloorToInt(zombie.pos.x)), 0, World.toChunkXZ(Mathf.FloorToInt(zombie.pos.z))) as Chunk;
            if (chunk == null)
            {
                Logger.Debug("Chunk not loaded at {0} {1}", zombie.pos, zombie.pos.z);
                return false;
            }

            int height = world.GetTerrainHeight(Mathf.FloorToInt(zombie.pos.x), Mathf.FloorToInt(zombie.pos.z));

            Vector3 spawnPos = new Vector3(zombie.pos.x, height + 1.0f, zombie.pos.z);
            if (!CanZombieSpawnAt(spawnPos))
            {
                Logger.Debug("Unable to spawn zombie at {0}, CanMobsSpawnAtPos failed", spawnPos);
                return false;
            }

            if (zombie.classId == -1)
            {
                zombie.classId = _biomeData.GetZombieClass(world, chunk, (int)spawnPos.x, (int)spawnPos.z, _prng);
                if (zombie.classId == -1)
                {
                    int lastClassId = -1;
                    zombie.classId = EntityGroups.GetRandomFromGroup("ZombiesAll", ref lastClassId);
                    Logger.Debug("Used fallback for zombie class!");
                }
            }

            if (EntityFactory.CreateEntity(zombie.classId, spawnPos) is not EntityZombie zombieEnt)
            {
                Logger.Error("Unable to create zombie entity!, Entity Id: {0}, Pos: {1}", zombie.classId, spawnPos);
                return false;
            }

            zombieEnt.bIsChunkObserver = true;

            // TODO: Figure out a better way to make them walk towards something.
            // Send zombie towards a random position in the zone.
            var targetPos = GetRandomZonePos(zone);
            if (targetPos == null)
            {
                Logger.Error("Had to send zombie to center zone.");
                targetPos = zone.center;
            }
            zombieEnt.SetInvestigatePosition(targetPos.Value, 6000, false);

            // If the zombie was previously damaged take health to this one.
            if (zombie.health != -1)
                zombieEnt.Health = zombie.health;
            else
                zombie.health = zombieEnt.Health;

            zombieEnt.IsHordeZombie = true;
            zombieEnt.IsBloodMoon = _state.IsBloodMoon;

            zombieEnt.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);

            world.SpawnEntityInWorld(zombieEnt);

            var active = zombie.MakeActive();
            active.entityId = zombieEnt.entityId;
            active.currentZone = zone;
            active.lifeTime = world.GetWorldTime();
            active.intendedGoal = targetPos.Value;

            zone.numZombies++;

            Logger.Debug("[{0}] Spawned zombie {1} at {2}", zombie.id, zombieEnt, spawnPos);
            lock (_activeZombies)
            {
                _activeZombies.Add(zombie);
            }

            return true;
        }
    }
}