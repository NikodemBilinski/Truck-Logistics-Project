namespace TrucksLogisticsClient.Pages;

using System.Diagnostics;
using System.Net.Http.Json;
using TrucksLogisticsClient.Models;

[QueryProperty(nameof(UserID), "UserID")]
public partial class MainMenuPage : ContentPage
{
    public int UserID { get; set; }

    private bool isUserDataFetched = false;
    public Users CurrentUser { get; set; }

    private string apiUrl = "http://192.168.0.218:5160/api/Values/";

    private HttpClient client = new HttpClient();

    public MainMenuPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Get_Current_User();
        if (isUserDataFetched)
        {
            await Generate_Main_Menu();
        }
        else
        {
            WelcomeUserLabel.Text = "Some error, check debug";
        }

    }

    private async Task Get_Current_User()
    {

        try
        {
            var response = await client.GetAsync(apiUrl + "Get_User_By_ID" + UserID);

            if (response != null)
            {
                var result = await response.Content.ReadFromJsonAsync<Users>();

                if (result != null)
                {
                    //double checking
                    CurrentUser = result;
                    isUserDataFetched = true;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error fetching user data: " + ex.Message);
        }
        

    }

    private async Task Generate_Main_Menu()
    {
        WelcomeUserLabel.Text = CurrentUser.Username;
    }
}