using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Media.Imaging;
using DynamicData;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VolatileHordes.Dto;
using VolatileHordes.GUI.Extensions;

namespace VolatileHordes.GUI.ViewModels
{
    public class PlayerVm : ViewModel
    {
        public int EntityId { get; }
        [Reactive] public RectangleF SpawnRectangle { get; private set; }
        [Reactive] public float Rotation { get; private set; }
        
        public IObservable<BitmapImage> Bitmaps { get; }

        public delegate PlayerVm Factory(int entityId);

        public PlayerVm(
            WorldstateVm worldstateVm,
            int entityId)
        {
            EntityId = entityId;
            Bitmaps = worldstateVm.ZombieGroups.Connect()
                .RemoveKey()
                .TransformMany(x => x.Zombies.Connect().RemoveKey().AsObservableList())
                .FilterOnObservable(zombie => Observable.CombineLatest(
                    zombie.WhenAnyValue(x => x.Position),
                    this.WhenAnyValue(x => x.SpawnRectangle),
                    (zombiePos, spawnRectangle) => spawnRectangle.Contains(zombiePos)))
                .AutoRefresh()
                .Debounce(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .QueryWhenChanged(zombiesInRange =>
                {
                    return GetBitmap(400, zombiesInRange);
                })
                .PublishRefCount();
        }

        public void Absorb(PlayerDto dto)
        {
            SpawnRectangle = dto.SpawnRectangle;
            Rotation = dto.Rotation % 360;
        }

        public BitmapImage GetBitmap(
            ushort size,
            IEnumerable<ZombieVm> zombies)
        {
            var bm = new Bitmap(size, size);
            using var gr = System.Drawing.Graphics.FromImage(bm);
            gr.Clear(System.Drawing.Color.Black);
            gr.FillPie(new SolidBrush(Color.FromArgb(50, 255, 255, 255)), new Rectangle(new Point(175, 175), new Size(50, 50)), Rotation - 35, 70);
            gr.FillEllipse(new SolidBrush(Color.Teal), 198, 198, 4, 4);
            return bm.ToBitmapImage();
        }
    }
}