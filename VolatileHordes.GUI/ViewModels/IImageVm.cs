using System.Windows.Media.Imaging;

namespace VolatileHordes.GUI.ViewModels
{
    public interface IImageVm
    {
        BitmapImage? Bitmap { get; }
        string Title { get; }
    }
}