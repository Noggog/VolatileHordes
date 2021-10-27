using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Media.Imaging;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VolatileHordes.GUI.Services;

namespace VolatileHordes.GUI.ViewModels
{
    public class AllocationVm : ViewModel, IImageVm
    {
        private readonly Subject<float[,]?> _allocations = new();
        
        private readonly ObservableAsPropertyHelper<BitmapImage?> _bitmap;
        public BitmapImage? Bitmap => _bitmap.Value;
        public string Title => "Allocation";

        [Reactive] public ushort Size { get; set; } = 400;

        public delegate AllocationVm Factory();
        
        public AllocationVm(DrawAllocationMap draw)
        {
            _bitmap = _allocations
                .Debounce(TimeSpan.FromMilliseconds(150), RxApp.MainThreadScheduler)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x => new DrawAllocationMapInput(Size, x))
                .Select(draw.Draw)
                .ToGuiProperty(this, nameof(Bitmap), default);
        }

        public void Set(float[,]? buckets)
        {
            _allocations.OnNext(buckets);
        }
    }
}