using System.Net.Http.Json;
using TrucksLogisticsClient.Models;

namespace TrucksLogisticsClient.Pages;

[QueryProperty(nameof(UserID), "UserID")]
public partial class UserMainMenuPage : ContentPage
{
	public int UserID { get; set; }

	public HttpClient client = new HttpClient();

    private string apiUrl = "http://192.168.0.218:5160/api/Values/";

    public Users? CurrentUser { get; set; }
    public UserMainMenuPage()
	{
		InitializeComponent();
		
	}

	public async Task HideEverything()
	{
		User_Show_Data_View.IsVisible = false;
		User_Show_Data_View.IsEnabled = false;

        User_Show_Trucks_View.IsVisible = false;
        User_Show_Trucks_View.IsEnabled = false;

        Jobs_View.IsVisible = false;
        Jobs_View.IsEnabled = false;

        User_Show_Chosen_Job.IsVisible = false;
		User_Show_Chosen_Job.IsEnabled = false;
    }
	public async Task<bool> GetUser()
	{
		var response = await client.GetAsync(apiUrl + "Get_User_By_ID/" + UserID);

		if (response.IsSuccessStatusCode)
		{
			CurrentUser = await response.Content.ReadFromJsonAsync<Users>();

			if (CurrentUser == null)
			{
				User_Error_Label_Main.Text = "Error: Current User is null";
				return false;
			}

		}
		return true;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

		bool GotUser = await GetUser();

		if (GotUser)
		{
			this.BindingContext = CurrentUser;
		}
        
    }

    private async void User_Show_Data(object sender, EventArgs e)
    {
		await HideEverything();
		User_Show_Data_View.IsVisible = true;
		User_Show_Data_View.IsEnabled = true;
    }

	private async void User_Show_Trucks(object sender, EventArgs e)
	{
		await HideEverything();

		User_Show_Trucks_View.IsEnabled = true;
        User_Show_Trucks_View.IsVisible = true;

		if (CurrentUser != null)
		{
            User_Show_Trucks_View_Collection.ItemsSource = null;
            User_Show_Trucks_View_Collection.ItemsSource = CurrentUser.AssignedTrucks;
		}
	}

	private async void User_Show_Assigned_Jobs(object sender, EventArgs e)
	{
		await HideEverything();

		Jobs_View.IsVisible = true;
		Jobs_View.IsEnabled = true;

		if (CurrentUser != null)
		{
			Jobs_View_Collection.ItemsSource = CurrentUser.AssignedJobs;
		}
	}

	private async void User_Show_Available_Jobs(object sender, EventArgs e)
	{
		
	}

	private async void User_Jobs_View_Selected(object sender, SelectionChangedEventArgs e)
	{
		await HideEverything();
		var selectedjob = e.CurrentSelection as Job;

		// tu skonczylem ostatnio :)
	}
}