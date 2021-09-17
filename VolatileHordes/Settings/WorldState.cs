﻿using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using UniLinq;
using VolatileHordes.ActiveDirectors;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes
{
    public class ZombieState
    {
        public int EntityId { get; set; }
    }

    public class ZombieGroupState
    {
        public ZombieState[] Zombies { get; set; } = new ZombieState[0];
        public PointF? Target { get; set; }
    }

    public class WorldState
    {
        public static readonly string StateFilePath =
            Path.Combine(GameUtils.GetSaveGameDir(), $"{Constants.ModName}State.json");

        public ZombieGroupState[] ZombieGroups { get; set; } = new ZombieGroupState[0];

        public static void Save()
        {
            Logger.Info("Saving state to {0}", StateFilePath);
            var state = new WorldState();
            state.ZombieGroups = Container.Director.Groups
                .Where(g => g.Zombies.Count > 0)
                .Select(g => new ZombieGroupState()
                {
                    Zombies = g.Zombies
                        .Select(z => new ZombieState()
                        {
                            EntityId = z.Id
                        })
                        .ToArray(),
                    Target = g.Target,
                })
                .ToArray();
            File.WriteAllText(StateFilePath, JsonConvert.SerializeObject(state, Formatting.Indented));
        }

        public static void Load()
        {
            Logger.Info("Loading state from {0}", StateFilePath);
            if (!File.Exists(StateFilePath)) return;
            var readIn = JsonConvert.DeserializeObject<WorldState>(File.ReadAllText(StateFilePath));
            if (readIn == null) return;

            foreach (var groupSettings in readIn.ZombieGroups)
            {
                var group = Container.Director.NewGroup();
                group.Zombies.AddRange(
                    groupSettings.Zombies
                        .Select<ZombieState, IZombie>(z => new Zombie(Container.World, z.EntityId))
                        .ToArray());
                group.Target = groupSettings.Target;
            }
        }
    }
}