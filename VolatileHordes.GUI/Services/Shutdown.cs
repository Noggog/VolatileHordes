using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using VolatileHordes.GUI.ViewModels;
using VolatileHordes.GUI.Views;

namespace VolatileHordes.GUI.Services
{
    public class Shutdown
    {
        private readonly ILifetimeScope _scope;
        private readonly SaveSettings _saveSettings;
        private readonly IMainWindow _window;
        
        public bool IsShutdown { get; private set; }

        public Shutdown(
            ILifetimeScope scope,
            SaveSettings saveSettings,
            IMainWindow window)
        {
            _scope = scope;
            _saveSettings = saveSettings;
            _window = window;
        }

        public void Prepare()
        {
            _window.Closing += (_, b) =>
            {
                _window.Visibility = Visibility.Collapsed;
                Closing(b);
            };
        }

        private void Closing(CancelEventArgs args)
        {
            if (IsShutdown) return;
            args.Cancel = true;
            ExecuteShutdown();
        }
        
        private async void ExecuteShutdown()
        {
            IsShutdown = true;

            await Task.Run(() =>
            {
                _saveSettings.Save();
            });
            
            var toDo = new List<Task>();

            toDo.Add(Task.Run(() =>
            {
                _scope.Dispose();
            }));
            await Task.WhenAll(toDo);
            Application.Current.Shutdown();
        }
    }
}