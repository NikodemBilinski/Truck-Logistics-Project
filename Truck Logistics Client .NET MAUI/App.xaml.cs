using Microsoft.Extensions.DependencyInjection;

namespace TrucksLogisticsClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            const int width = 1200;
            const int height = 500;

            window.MinimumWidth = width;
            window.MinimumHeight = height;

            window.Height = 800;


            return window;

            //return new Window(new AppShell());
        }
    }
}