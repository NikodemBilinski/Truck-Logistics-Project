using TrucksLogisticsClient.Pages;

namespace TrucksLogisticsClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Pages.MainMenuPage), typeof(Pages.MainMenuPage));
            Routing.RegisterRoute(nameof(Pages.UserMainMenuPage), typeof(Pages.UserMainMenuPage));
            Routing.RegisterRoute(nameof(Pages.AdminInvoicesAndClientsPage), typeof(Pages.AdminInvoicesAndClientsPage));
            Routing.RegisterRoute(nameof(Pages.AdminUsersAndTrucksPage), typeof(Pages.AdminUsersAndTrucksPage));
            Routing.RegisterRoute(nameof(Pages.AdminJobsPage), typeof(Pages.AdminJobsPage));
        }
    }
}
