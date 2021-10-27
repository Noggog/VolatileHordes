using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using Noggog.WPF;
using ReactiveUI;

namespace VolatileHordes.GUI.ViewModels
{
    public class MainVm : ViewModel
    {
        public ConnectionVm Connection { get; }
        
        public MainSettingsVm Settings { get; }

        public IObservableCollection<PlayerVm> Players { get; }

        public IObservableCollection<IImageVm> ImageDisplays { get; }
        
        public LimitsVm Limits { get; }

        public MainVm(
            ConnectionVm connectionVm,
            MainSettingsVm settingsVm,
            WorldstateVm worldstateVm)
        {
            Connection = connectionVm;
            Settings = settingsVm;

            Limits = worldstateVm.Limits;

            Players = worldstateVm.Players.Connect()
                .ToObservableCollection(this);

            ImageDisplays = worldstateVm.AllocationVm.AsEnumerable<IImageVm>().AsObservableChangeSet()
                .Or(
                    worldstateVm.Players.Connect()
                        .AutoRefresh(x => x.Display, scheduler: RxApp.MainThreadScheduler)
                        .Filter(x => x.Display)
                        .Transform(x => (IImageVm)x)
                        .RemoveKey())
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToObservableCollection(this);
        }

        public void Init()
        {
        }
    }
}