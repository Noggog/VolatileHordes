using System.Linq;
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

        public IObservableCollection<PlayerVm> DisplayPlayers { get; }

        public MainVm(
            ConnectionVm connectionVm,
            MainSettingsVm settingsVm,
            WorldstateVm worldstateVm)
        {
            Connection = connectionVm;
            Settings = settingsVm;

            Players = worldstateVm.Players.Connect()
                .ToObservableCollection(this);

            DisplayPlayers = worldstateVm.Players.Connect()
                .AutoRefresh(x => x.Display, scheduler: RxApp.MainThreadScheduler)
                .Filter(x => x.Display)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToObservableCollection(this);
        }

        public void Init()
        {
        }
    }
}