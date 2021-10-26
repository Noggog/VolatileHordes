using Noggog.WPF;
using ReactiveUI.Fody.Helpers;
using VolatileHordes.GUI.Services;

namespace VolatileHordes.GUI.ViewModels
{
    public class MainSettingsVm : ViewModel, ISettingsTarget
    {
        [Reactive] public bool DrawGroupTargets { get; set; }
        [Reactive] public bool DrawTargets { get; set; }
        
        public void Load(Settings settings)
        {
            DrawGroupTargets = settings.DrawGroupTargets;
            DrawTargets = settings.DrawTargets;
        }

        public void Save(Settings settings)
        {
            settings.DrawTargets = DrawTargets;
            settings.DrawGroupTargets = DrawGroupTargets;
        }
    }
}