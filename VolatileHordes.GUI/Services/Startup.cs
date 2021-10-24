using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using VolatileHordes.GUI.ViewModels;
using VolatileHordes.GUI.Views;

namespace VolatileHordes.GUI.Services
{
    public class Startup
    {
        private readonly IEnumerable<IStartupTask> _startupTasks;
        private readonly IMainWindow _window;
        private readonly Lazy<MainVm> _mainVm;
        private readonly Shutdown _shutdown;

        public Startup(
            IEnumerable<IStartupTask> startupTasks,
            IMainWindow window,
            Lazy<MainVm> mainVm,
            Shutdown shutdown)
        {
            _startupTasks = startupTasks;
            _window = window;
            _mainVm = mainVm;
            _shutdown = shutdown;
        }
        
        public async void Initialize()
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            {
            };
            
            try
            {
                await Observable.FromAsync(() =>
                        Task.WhenAll(_startupTasks
                            .Select(x => Task.Run(x.Start))))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(_ =>
                    {
                        var mainVM = _mainVm.Value;
                        _window.DataContext = mainVM;
                        mainVM.Init();
                    });
            }
            catch (Exception e)
            {
                Application.Current.Shutdown();
            }
            
            _shutdown.Prepare();
        }
    }
}