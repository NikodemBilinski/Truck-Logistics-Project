using System.Diagnostics;
using System.Net.Http.Json;
using TrucksLogisticsClient.Models;

namespace TrucksLogisticsClient.Pages;

[QueryProperty(nameof(UserID), "UserID")]
public partial class AdminUsersAndTrucksPage : ContentPage
{
	public int UserID { get; set; }

    private bool isUserDataFetched = false;

    public Users? CurrentUser { get; set; }

	private List<Language> SelectedLanguages = new List<Language>();

	private List<Truck> SelectedTrucks = new List<Truck>();

	private string apiUrl = "http://192.168.0.218:5160/api/";

	private HttpClient client = new HttpClient();
	public AdminUsersAndTrucksPage()
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
            var response = await client.GetAsync(apiUrl + "Users/Get_User_By_ID/" + UserID);

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

    private async Task Hide_Everything()
    {
        Users_View.IsVisible = false;
        Users_View.IsEnabled = false;

        Trucks_View.IsVisible = false;
        Trucks_View.IsEnabled = false;

        //Jobs_View.IsVisible = false;
        //Jobs_View.IsEnabled = false;

        //Edit_User_Section.IsVisible = false;
        //Edit_User_Section.IsEnabled = false;

        //Edit_Truck_Section.IsVisible = false;
        //Edit_Truck_Section.IsEnabled = false;

        //Edit_Job_Section.IsVisible = false;
        //Edit_Job_Section.IsEnabled = false;

        //Add_User_Section.IsVisible = false;
        //Add_User_Section.IsEnabled = false;

        //Add_Truck_Section.IsVisible = false;
        //Add_Truck_Section.IsEnabled = false;

        //Add_Job_Section.IsVisible = false;
        //Add_Job_Section.IsEnabled = false;


    }

    // GET Users, Trucks

    private async void Admin_Get_Users_Clicked(object sender, EventArgs e)
    {
        await Hide_Everything();
        try
        {
            var response = await client.GetAsync(apiUrl + "Users/Get_All_Users");

            if (response.IsSuccessStatusCode)
            {
                var userslist = await response.Content.ReadFromJsonAsync<List<Users>>();

                Get_All_Users_View.ItemsSource = userslist;


            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
            return;
        }
        Users_View.IsEnabled = true;
        Users_View.IsVisible = true;
    }

    private async void Admin_Get_Trucks_Clicked(object sender, EventArgs e)
    {
        await Hide_Everything();

        try
        {
            var response = await client.GetAsync(apiUrl + "Trucks/Get_Trucks");
            if (response.IsSuccessStatusCode)
            {
                var truckslist = await response.Content.ReadFromJsonAsync<List<Truck>>();

                Get_All_Trucks_View.ItemsSource = truckslist;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
            return;
        }
        Trucks_View.IsEnabled = true;
        Trucks_View.IsVisible = true;
    }

    //Open Certain Sections

    private async void Admin_Users_View_Selected(object sender, SelectionChangedEventArgs e)
    {

    }

    private async void Admin_Trucks_View_Selected(object sender, SelectionChangedEventArgs e)
    {

    }


}