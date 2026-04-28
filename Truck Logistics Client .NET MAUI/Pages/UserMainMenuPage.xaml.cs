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
}