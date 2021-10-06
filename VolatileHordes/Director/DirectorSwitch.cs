using System.Reactive.Subjects;
using VolatileHordes.Settings.User.Director;

namespace VolatileHordes.Director
{
    public class DirectorSwitch
    {
        public BehaviorSubject<bool> Enabled { get; }

        public DirectorSwitch(
            DirectorSettings settings)
        {
            Enabled = new BehaviorSubject<bool>(settings.Enabled);
            Logger.Info("Directors {0}", settings.Enabled ? "enabled" : "disabled");
        }
    }
}