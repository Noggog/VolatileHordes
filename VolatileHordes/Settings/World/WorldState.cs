using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using UniLinq;
using VolatileHordes.Settings.World.Zombies;

namespace VolatileHordes.Settings.World
{
    public class WorldState
    {
        public static readonly string StateFilePath =
            Path.Combine(GameUtils.GetSaveGameDir(), $"{Constants.ModName}State.json");

        public ZombieGroupState[] ZombieGroups { get; set; } = new ZombieGroupState[0];

        public static readonly JsonSerializerSettings JsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        public static void Save()
        {
            Logger.Info("Saving state to {0}", StateFilePath);
            var state = new WorldState();
            state.ZombieGroups = Container.GroupManager.Groups
                .Select(g => ZombieGroupState.GetSettings(g))
                .NotNull()
                .ToArray();
            File.WriteAllText(StateFilePath, JsonConvert.SerializeObject(state, Formatting.Indented, JsonSettings));
        }

        public static void Load()
        {
            Logger.Info("Loading state from {0}", StateFilePath);
            if (!File.Exists(StateFilePath)) return;
            var readIn = JsonConvert.DeserializeObject<WorldState>(File.ReadAllText(StateFilePath), JsonSettings);
            if (readIn == null) return;

            foreach (var groupSettings in readIn.ZombieGroups)
            {
                groupSettings.ApplyToWorld();
            }
        }
    }
}