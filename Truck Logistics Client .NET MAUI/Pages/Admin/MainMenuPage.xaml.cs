namespace TrucksLogisticsClient.Pages;

using System.Diagnostics;
using System.Net.Http.Json;
using TrucksLogisticsClient.Models;
using TrucksLogisticsClient.Models.Helping_Models;

[QueryProperty(nameof(UserID), "UserID")]
public partial class MainMenuPage : ContentPage
{
    public int UserID { get; set; }

    private bool isUserDataFetched = false;
    public Users? CurrentUser { get; set; }

    private List<Language> SelectedLanguages = new List<Language>();

    private List<Truck> SelectedTrucks = new List<Truck>();

    private string apiUrl;

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

        var token = await SecureStorage.GetAsync("auth_token");

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        apiUrl = Preferences.Get("api_url", "127.0.0.1:5160/api/");

        await Get_Current_User();

        await Generate_MainMenu();
    }

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

    private async Task Generate_MainMenu()
    {

        var response = await client.GetAsync(apiUrl + "Invoices/Get_Invoices_Stats");

        if(response.IsSuccessStatusCode)
        {

            var stats = await response.Content.ReadFromJsonAsync<InvoicesStats>();

            if(stats != null)
            {
                Unpaid_Invoices_Count.Text = stats.Unpaid_Count.ToString();
                if (stats.Unpaid_Count > 0)
                {
                    Unpaid_Invoices_Count.TextColor = Colors.Orange;
                }

                Overdue_Invoices_Count.Text += stats.Overdue_Count.ToString();
                if (stats.Overdue_Count > 0)
                {
                    Overdue_Invoices_Count.TextColor = Colors.DeepPink;
                }
            }
            
        }

        Welcome_User_Date.Text = DateTime.Now.ToString("dddd") + ", " + DateTime.Now.ToString("d");
    }

    // page navigation
    
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

    private async void Admin_LogOut(object sender, EventArgs e)
    {
        // Clear the token from secure storage
        SecureStorage.Remove("auth_token");

        await Shell.Current.GoToAsync("//MainPage");
    }



}