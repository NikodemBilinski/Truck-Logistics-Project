using System.Diagnostics;
using System.Net.Http.Json;
using TrucksLogisticsClient.Models;

namespace TrucksLogisticsClient.Pages;

[QueryProperty(nameof(UserID), "UserID")]
public partial class AdminInvoicesAndClientsPage : ContentPage
{
	public int UserID { get; set; }

    private bool isUserDataFetched;

    public Users? CurrentUser { get; set; }

    private string apiUrl = "http://192.168.0.218:5160/api/Values/";

    private HttpClient client = new HttpClient();

    public AdminInvoicesAndClientsPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Get_Current_User();
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

                    Welcome_User_Label.Text = CurrentUser.Username;
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

    private async Task HideEverything()
    {
        Add_Client_Section.IsVisible = false;
        Add_Client_Section.IsEnabled = false;
    }
    // open sections
    private async void Admin_Open_Add_Client_Section(object sender, EventArgs e)
    {
        await HideEverything();

        Add_Client_Section.IsVisible = true;
        Add_Client_Section.IsEnabled = true;
    }
    private async void Admin_Open_Add_Invoice_Section(object sender, EventArgs e)
    {

    }

    // add to database 
    private async void Admin_Add_Client_Clicked(object sender, EventArgs e)
    {

    }
    private async void Admin_Go_Back(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

}