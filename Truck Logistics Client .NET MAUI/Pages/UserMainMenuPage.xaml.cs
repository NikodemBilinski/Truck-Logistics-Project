using System.Diagnostics;
using System.Net.Http.Json;
using TrucksLogisticsClient.Models;

namespace TrucksLogisticsClient.Pages;

[QueryProperty(nameof(UserID), "UserID")]
public partial class UserMainMenuPage : ContentPage
{
	public int UserID { get; set; }

	public HttpClient client = new HttpClient();

    private string apiUrl = "http://192.168.0.218:5160/api/Values/";

	private List<Language> SelectedLanguages = new List<Language>();

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

		Edit_User_Section.IsVisible = false;
        Edit_User_Section.IsEnabled = false;
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
            Jobs_View_Collection.SelectedItem = null;

        }

    }

	private async void User_Show_Available_Jobs(object sender, EventArgs e)
	{
		await HideEverything();

        Jobs_View.IsVisible = true;
        Jobs_View.IsEnabled = true;

		var response = await client.GetAsync(apiUrl + "Get_Open_Jobs");

		if(response.IsSuccessStatusCode)
		{
			var allopenjobs = await response.Content.ReadFromJsonAsync<List<Job>>();

            if (allopenjobs != null)
            {
				Jobs_View_Collection.ItemsSource = allopenjobs;
                Jobs_View_Collection.SelectedItem = null;
            }
        }
    }

	private async void User_Jobs_View_Selected(object sender, SelectionChangedEventArgs e)
	{
		
		var selectedjob = e.CurrentSelection.FirstOrDefault() as Job;

		if(selectedjob == null)
		{
			return;
		}
		if (selectedjob != null)
		{
            await HideEverything();
            User_Show_Chosen_Job.IsVisible = true;
            User_Show_Chosen_Job.IsEnabled = true;
            User_Show_Chosen_Job.BindingContext = selectedjob;
        }

    }

	private async void Show_User_Edit(object sender, EventArgs e)
	{
		await HideEverything();



        if (CurrentUser == null)
		{
			return;
		}

		var response = await client.GetAsync(apiUrl + "Get_Languages");

		if(response.IsSuccessStatusCode)
		{
			var AllLanguages = await response.Content.ReadFromJsonAsync<List<Language>>();

            SelectedLanguages.Clear();
            foreach (var lang in AllLanguages)
            {
				if(CurrentUser.Languages.Any(x=> x.Id == lang.Id))
				{
                    lang.SelectionColor = Colors.LightBlue;
                    SelectedLanguages.Add(lang);
                }
				else
				{
					lang.SelectionColor = Colors.Transparent;
				}
                
            }
			All_Languages_View.ItemsSource = null;
            All_Languages_View.ItemsSource = AllLanguages;
        }

        Edit_User_Section.BindingContext = CurrentUser;

		Edit_User_Section.IsVisible = true;
		Edit_User_Section.IsEnabled = true;
	}

	private async void Save_User_Edit(object sender, EventArgs e)
	{
		var selecteduser = Edit_User_Section.BindingContext as Users;

		//reset selectedlanguages
		
	}

	private async void On_Language_Tapped(object sender, EventArgs e)
	{
		var border = (Border)sender;

		var language = (Language)sender as Language;

		if (language != null)
		{
			if(SelectedLanguages.Contains(language))
			{
				language.SelectionColor = Colors.Transparent;
				SelectedLanguages.Remove(language);
			}
            else if(!SelectedLanguages.Contains(language))
            {
                language.SelectionColor= Colors.LightBlue;
				SelectedLanguages.Add(language);
            }
        }
	}
}