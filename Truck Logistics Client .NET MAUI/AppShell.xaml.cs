using TrucksLogisticsClient.Pages;

namespace TrucksLogisticsClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainMenuPage), typeof(MainMenuPage));
        }
    }
}
