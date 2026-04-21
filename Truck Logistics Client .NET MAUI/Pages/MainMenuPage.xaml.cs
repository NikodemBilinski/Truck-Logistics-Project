namespace TrucksLogisticsClient.Pages;

using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        if(isUserDataFetched)
        {
            this.BindingContext = CurrentUser;
            await Generate_Main_Menu();
        }
        

    }

    private async Task Get_Current_User()
    {

        try
        {
            var response = await client.GetAsync(apiUrl + "Get_User_By_ID/" + UserID);

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
        if(CurrentUser.Role == "admin")
        {
            User_Get_Trucks.IsEnabled = false;
            User_Get_Trucks.IsVisible = false;

            Admin_Get_Trucks.IsEnabled = true;
            Admin_Get_Trucks.IsVisible = true;

            Admin_Get_Users.IsEnabled = true;
            Admin_Get_Users.IsVisible = true;
            
        }
        if (CurrentUser.Role == "user")
        {
            User_Get_Trucks.IsEnabled = true;
            User_Get_Trucks.IsVisible = true;

            Admin_Get_Trucks.IsEnabled = false;
            Admin_Get_Trucks.IsVisible = false;

        }
    }

    private async void Admin_Get_Users_Clicked(object sender, EventArgs e)
    {
        try
        {
            var response = await client.GetAsync(apiUrl + "Get_All_Users");

            if(response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var userslist = JsonSerializer.Deserialize<List<Users>>(json, options);

                Get_All_Users_View.ItemsSource = userslist;
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
            return;
        }

        Users_View.IsVisible = true;
    }
}