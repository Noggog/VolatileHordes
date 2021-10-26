using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using UniLinq;
using VolatileHordes.Settings.World.Zombies;

namespace VolatileHordes.Settings.World
{
    public class WorldStateSettings
    {
        public static readonly string StateFilePath =
            Path.Combine(GameUtils.GetSaveGameDir(), $"{Constants.ModName}State.json");

        public ZombieGroupState[] ZombieGroups { get; set; } = new ZombieGroupState[0];
        
        public AllocationWorldSettings Allocation { get; set; } = new();

        public static readonly JsonSerializerSettings JsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        public static void Save()
        {
            Logger.Info("Saving state to {0}", StateFilePath);
            var state = new WorldStateSettings();
            state.ZombieGroups = Container.ZombieGroupManager.NormalGroups
                .Select(g => ZombieGroupState.GetSettings(g))
                .NotNull()
                .ToArray();

            Container.AllocationBuckets.Save(state.Allocation);
            
            File.WriteAllText(StateFilePath, JsonConvert.SerializeObject(state, Formatting.Indented, JsonSettings));
        }

        public static WorldStateSettings Load()
        {
            Logger.Info("Loading state from {0}", StateFilePath);
            if (!File.Exists(StateFilePath)) return new();
            var readIn = JsonConvert.DeserializeObject<WorldStateSettings>(File.ReadAllText(StateFilePath), JsonSettings);
            if (readIn == null) return new();

            foreach (var groupSettings in readIn.ZombieGroups)
            {
                groupSettings.ApplyToWorld();
            }
            
            Container.AllocationBuckets.Init(readIn.Allocation);
            return readIn;
        }
    }
}