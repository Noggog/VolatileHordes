using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media.Imaging;
using DynamicData;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VolatileHordes.Dto;
using VolatileHordes.GUI.Services;

namespace VolatileHordes.GUI.ViewModels
{
    public class PlayerVm : ViewModel
    {
        public int EntityId { get; }
        [Reactive] public RectangleF Rectangle { get; private set; }
        [Reactive] public RectangleF SpawnRectangle { get; private set; }
        [Reactive] public float Rotation { get; private set; }
        [Reactive] public bool Display { get; set; }
        [Reactive] public string Name { get; set; } = string.Empty;

        [Reactive] public ushort Size { get; set; } = 400;
        
        private readonly ObservableAsPropertyHelper<BitmapImage?> _bitmap;
        public BitmapImage? Bitmap => _bitmap.Value;

        public delegate PlayerVm Factory(int entityId);

        public PlayerVm(
            WorldstateVm worldstateVm,
            DrawPlayerZone drawPlayer,
            MainSettingsVm mainSettingsVm,
            int entityId)
        {
            EntityId = entityId;
            _bitmap = worldstateVm.ZombieGroups.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .RemoveKey()
                .TransformMany(x => x.Zombies.Connect().RemoveKey().AsObservableList())
                .FilterOnObservable(zombie => Observable.CombineLatest(
                    zombie.WhenAnyValue(x => x.Position),
                    this.WhenAnyValue(x => x.Rectangle),
                    (zombiePos, rect) => rect.Contains(zombiePos)))
                .AutoRefresh()
                .QueryWhenChanged(x => x)
                .StartWithEmpty()
                .CombineLatest(
                    this.WhenAnyValue(x => x.Size),
                    this.WhenAnyValue(x => x.Rotation),
                    (zombiesInRange, _, _) =>
                    {
                        return new DrawInput(
                            new PlayerDrawInput(
                                this.SpawnRectangle, 
                                this.Rectangle,
                                this.Rotation),
                            Size,
                            zombiesInRange.Select(z => new ZombieDrawInput(z.Position, z.Target, z.GroupVm.Target, z.Rotation)).ToArray(),
                            DrawTargetLines: mainSettingsVm.DrawTargets,
                            DrawGroupTargetLines: mainSettingsVm.DrawGroupTargets);
                    })
                .Debounce(TimeSpan.FromMilliseconds(150), RxApp.MainThreadScheduler)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(drawPlayer.Draw)
                .FilterSwitch(this.WhenAnyValue(x => x.Display))
                .ToGuiProperty(this, nameof(Bitmap), default);
        }

        public void Absorb(PlayerDto dto)
        {
            SpawnRectangle = dto.SpawnRectangle;
            Rectangle = dto.Rectangle;
            Rotation = dto.Rotation % 360;
            Name = dto.Name;
        }
    }
}