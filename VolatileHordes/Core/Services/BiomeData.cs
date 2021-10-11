using System.Collections.Generic;
using VolatileHordes.Probability;

namespace VolatileHordes
{
    public class BiomeData
    {
	    private readonly RandomSource _randomSource;
	    private DictionarySave<string, BiomeSpawnEntityGroupList> list = new();

		public BiomeData(RandomSource randomSource)
		{
			_randomSource = randomSource;
		}

		public void Init(bool clearOriginal = false)
		{
			var world = GameManager.Instance.World;

			foreach (var pair in world.Biomes.GetBiomeMap())
			{
				var biome = pair.Value;

				BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = BiomeSpawningClass.list[biome.m_sBiomeName];
				if (biomeSpawnEntityGroupList == null)
				{
					continue;
				}

				// Clearing the biome data prevents random zombie spawns
				// We make a copy to keep for ourselves for the simulation.
				var copy = new BiomeSpawnEntityGroupList();
				copy.list = new List<BiomeSpawnEntityGroupData>(biomeSpawnEntityGroupList.list);

				if (clearOriginal)
				{
					biomeSpawnEntityGroupList.list.Clear();
				}

				list.Add(biome.m_sBiomeName, copy);
			}
		}

		public int GetZombieClass(Chunk chunk, int x, int y)
		{
			var world = GameManager.Instance.World;
			
			ChunkAreaBiomeSpawnData spawnData = chunk.GetChunkBiomeSpawnData();
			if (spawnData == null)
			{
#if DEBUG
				Logger.Warning("No biome spawn data present");
#endif
				return -1;
			}

			var biomeData = world.Biomes.GetBiome(spawnData.biomeId);
			if (biomeData == null)
			{
#if DEBUG
				Logger.Warning("No biome data for biome id {0}", spawnData.biomeId);
#endif
				return -1;
			}

			BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = list[biomeData.m_sBiomeName];
			if (biomeSpawnEntityGroupList == null)
			{
#if DEBUG
				Logger.Warning("No biome spawn group specified for {0}", biomeData.m_sBiomeName);
#endif
				return -1;
			}

			var numGroups = biomeSpawnEntityGroupList.list.Count;
			if (numGroups == 0)
			{
#if DEBUG
				Logger.Warning("Biome spawn group is empty for {0}", biomeData.m_sBiomeName);
#endif
				return -1;
			}

			var dayTime = world.IsDaytime() ? EDaytime.Day : EDaytime.Night;
			for (int i = 0; i < 5; i++)
			{
				int pickIndex = _randomSource.Get(0, numGroups);

				var pick = biomeSpawnEntityGroupList.list[pickIndex];
				if (pick.daytime == EDaytime.Any || pick.daytime == dayTime)
				{
					int lastClassId = -1;
					return EntityGroups.GetRandomFromGroup(pick.entityGroupRefName, ref lastClassId);
				}
			}

#if DEBUG
			Logger.Warning("No Biome spawn pick could be found for {0}, despite it having {1} items", biomeData.m_sBiomeName, biomeSpawnEntityGroupList.list.Count);
#endif
			return -1;
		}
	}
}
