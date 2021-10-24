using System.ComponentModel;
using System.Windows;
using Autofac;
using VolatileHordes.GUI.Services;
using VolatileHordes.GUI.ViewModels;

namespace VolatileHordes.GUI.Views
{
    public interface IMainWindow
    {
        public Visibility Visibility { get; set; }
        public object DataContext { get; set; }
        event CancelEventHandler Closing;
    }
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var builder = new ContainerBuilder();
            builder.RegisterModule<MainModule>();
            builder.RegisterInstance(this)
                .AsSelf()
                .As<IMainWindow>();
            var container = builder.Build();
            
            container.Resolve<Startup>()
                .Initialize();
        }
    }
}