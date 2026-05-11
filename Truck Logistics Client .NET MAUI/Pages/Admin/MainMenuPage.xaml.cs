namespace TrucksLogisticsClient.Pages;

using Microsoft.Maui.Graphics.Text;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrucksLogisticsClient.Models;

[QueryProperty(nameof(UserID), "UserID")]
public partial class MainMenuPage : ContentPage
{
    public int UserID { get; set; }

    private bool isUserDataFetched = false;
    public Users? CurrentUser { get; set; }

    private List<Language> SelectedLanguages = new List<Language>();

    private List<Truck> SelectedTrucks = new List<Truck>();

    private string apiUrl = "http://192.168.0.218:5160/api/";

    private HttpClient client = new HttpClient();

    public MainMenuPage()
    {
        InitializeComponent();

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsEnabled = false,
            IsVisible = false
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Get_Current_User();
    }

    //GET CURRENT USER, HIDE EVERYTHING, GET LANGUAGES
    private async Task Get_Current_User()
    {

        try
        {
            var response = await client.GetAsync(apiUrl + "Users/Get_User_By_ID/" + UserID);

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
            this.BindingContext = CurrentUser;

            Admin_Data_Panel.IsEnabled = true;
            Admin_Data_Panel.IsVisible = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error fetching user data: " + ex.Message);
            Welcome_User_Label.Text = "Error fetching user data: " + ex.Message;
        }
        

    }

    private async Task<List<Language>> Get_Languages()
    {
        try
        {
            var response = await client.GetAsync(apiUrl + "Values/Get_Languages");
    
            if(response.IsSuccessStatusCode)
            {
                var Languages = await response.Content.ReadFromJsonAsync<List<Language>>();
                
                if(Languages != null)
                {
                    return Languages;
                }  
                else
                {
                    return new List<Language>();
                }
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
            return new List<Language>();
        }
        return new List<Language>();
    }

    private async Task<List<Client>> Get_Clients()
    {
        try
        {
            var response = await client.GetAsync(apiUrl + "Get_Clients");

            if (response.IsSuccessStatusCode)
            {
                var Clients = await response.Content.ReadFromJsonAsync<List<Client>>();

                if (Clients != null)
                {
                    return Clients;
                }
                else
                {
                    return new List<Client>();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
            return new List<Client>();
        }
        return new List<Client>();
    }






    // other stuff
    private async void On_Language_Tapped(object sender, EventArgs e)
    {
        var border = (Border)sender;
        var tappedLanguage = (Language)border.BindingContext;

        // if language is selected, deselect it, else select it
        if (SelectedLanguages.Contains(tappedLanguage))
        {
            SelectedLanguages.Remove(tappedLanguage);
            border.BackgroundColor = Colors.Transparent;
        }
        else
        {
            SelectedLanguages.Add(tappedLanguage);
            border.BackgroundColor = Colors.LightBlue;
        }

    }

    private async void On_Truck_Tapped(object sender, EventArgs e)
    {
        var border = (Border)sender;

        var tappedtruck = (Truck)border.BindingContext;

        if (SelectedTrucks.Contains(tappedtruck))
        {
            SelectedTrucks.Remove(tappedtruck);
            border.BackgroundColor = Colors.Transparent;
        }
        else
        {
            SelectedTrucks.Add(tappedtruck);
            border.BackgroundColor = Colors.LightBlue;
        }

            
        
    }


    private async void Admin_MoveTo_InvoicesAndClients(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(AdminInvoicesAndClientsPage)}?UserID={UserID}");
    }

    private async void Admin_MoveTo_UsersAndTrucks(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(AdminUsersAndTrucksPage)}?UserID={UserID}");
    }

    private async void Admin_MoveTo_Jobs(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(AdminJobsPage)}?UserID={UserID}");
    }



}