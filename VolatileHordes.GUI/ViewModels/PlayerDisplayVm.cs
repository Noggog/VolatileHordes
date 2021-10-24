﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media.Imaging;
using DynamicData;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VolatileHordes.GUI.Services;

namespace VolatileHordes.GUI.ViewModels
{
    public class PlayerDisplayVm : ViewModel
    {
        public PlayerVm Player { get; }
        
        private readonly ObservableAsPropertyHelper<BitmapImage?> _bitmap;
        public BitmapImage? Bitmap => _bitmap.Value;

        public delegate PlayerDisplayVm Factory(PlayerVm pvm);

        [Reactive] public ushort Size { get; set; } = 400;

        public PlayerDisplayVm(
            WorldstateVm worldstateVm,
            DrawPlayerZone drawPlayer,
            PlayerVm pvm)
        {
            Player = pvm;
            _bitmap = worldstateVm.ZombieGroups.Connect()
                    .RemoveKey()
                    .TransformMany(x => x.Zombies.Connect().RemoveKey().AsObservableList())
                    .FilterOnObservable(zombie => Observable.CombineLatest(
                        zombie.WhenAnyValue(x => x.Position),
                        pvm.WhenAnyValue(x => x.SpawnRectangle),
                        (zombiePos, spawnRectangle) => spawnRectangle.Contains(zombiePos)))
                    .AutoRefresh()
                    .QueryWhenChanged(x => x)
                    .CombineLatest(
                        this.WhenAnyValue(x => x.Size),
                        pvm.WhenAnyValue(x => x.Rotation),
                        (zombiesInRange, _, _) => new DrawInput(pvm, Size, zombiesInRange.ToArray()))
                    .Debounce(TimeSpan.FromMilliseconds(150), RxApp.MainThreadScheduler)
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Select(drawPlayer.Draw)
                .ToGuiProperty(this, nameof(Bitmap), default);
        }
    }
}