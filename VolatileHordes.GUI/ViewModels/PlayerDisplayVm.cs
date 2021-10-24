using System.Drawing;
using System.Windows.Media.Imaging;
using Noggog.WPF;
using ReactiveUI;

namespace VolatileHordes.GUI.ViewModels
{
    public class PlayerDisplayVm : ViewModel
    {
        public PlayerVm Player { get; }
        
        private readonly ObservableAsPropertyHelper<BitmapImage?> _bitmap;
        public BitmapImage? Bitmap => _bitmap.Value;
        
        public PlayerDisplayVm(PlayerVm pvm)
        {
            Player = pvm;
            _bitmap = pvm.Bitmaps
                .ToGuiProperty(this, nameof(Bitmap), default);
        }
    }
}