namespace TrucksLogisticsClient.Pages;
using TrucksLogisticsClient.Models;

[QueryProperty(nameof(UserID), "UserID")]
public partial class MainMenuPage : ContentPage
{
    public int UserID { get; set; }
    public Users CurrentUser { get; set; }


    public MainMenuPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        WelcomeUserLabel.Text = "Welcome, " + UserID;
    }
}