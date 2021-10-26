using Autofac;
using Noggog.Autofac;
using VolatileHordes.GUI.Services;
using VolatileHordes.GUI.ViewModels;

namespace VolatileHordes.GUI
{
    public class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(Startup).Assembly)
                .InNamespacesOf(
                    typeof(Startup),
                    typeof(MainVm))
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}