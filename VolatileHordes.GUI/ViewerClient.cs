using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using VolatileHordes.Dto;

namespace VolatileHordes.GUI
{
    public class ViewerClient
    {
        public IObservable<State> States { get; }

        public ViewerClient(IPEndPoint endPoint)
        {
            States = Observable.Create<State>(async (obs, cancel) =>
            {
                var client = new TcpClient();
                client.Connect(endPoint);
                using var reader = new BinaryReader(client.GetStream());
                while (!cancel.IsCancellationRequested)
                {
                    var state = State.Deserialize(reader);
                
                    obs.OnNext(state);
                }
                
                return Disposable.Empty;
            });
        }
    }
}