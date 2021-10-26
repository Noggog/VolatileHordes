using System;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VolatileHordes.GUI.Services;

namespace VolatileHordes.GUI.ViewModels
{
    public class ConnectionVm : ViewModel, ISettingsTarget
    {
        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
        public ReactiveCommand<Unit, Unit> DisconnectCommand { get; }
        
        [Reactive] public byte First { get; set; }
        
        [Reactive] public byte Second { get; set; }
        
        [Reactive] public byte Third { get; set; }
        
        [Reactive] public byte Fourth { get; set; }
        
        [Reactive] public ushort Port { get; set; }
        
        [Reactive] public ViewerClient? Client { get; private set; }

        private readonly ObservableAsPropertyHelper<bool> _Connected;
        public bool Connected => _Connected.Value;

        public ConnectionVm()
        {
            _Connected = this.WhenAnyValue(x => x.Client)
                .Select(c => c != null)
                .ToGuiProperty(this, nameof(Connected));

            ConnectCommand = ReactiveCommand.Create(
                    ActionExt.Nothing,
                    canExecute: this.WhenAnyValue(x => x.Connected)
                        .Select(x => !x));
            DisconnectCommand = ReactiveCommand.Create(
                ActionExt.Nothing,
                canExecute: this.WhenAnyValue(x => x.Connected));

            ConnectCommand
                .WithLatestFrom(
                    this.WhenAnyValue(
                        x => x.First,
                        x => x.Second,
                        x => x.Third,
                        x => x.Fourth,
                        x => x.Port,
                        (f, s, t, ft, p) => new IPEndPoint(new IPAddress(new[] { f, s, t, ft }), p)),
                    (_, ip) => ip)
                .Select(ip => new ViewerClient(ip))
                .Merge(DisconnectCommand
                    .Select(_ => default(ViewerClient?)))
                .Subscribe(client => Client = client)
                .DisposeWith(this);
        }

        public void Load(Settings settings)
        {
            var ip = IPAddress.Parse(settings.Ip);
            var bytes = ip.GetAddressBytes();
            First = bytes[0];
            Second = bytes[1];
            Third = bytes[2];
            Fourth = bytes[3];
            Port = settings.Port;
        }

        public void Save(Settings settings)
        {
            settings.Ip = $"{First}.{Second}.{Third}.{Fourth}";
            settings.Port = Port;
        }
    }
}