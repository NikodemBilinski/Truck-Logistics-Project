using System.Diagnostics;
using System.Net.Http.Json;
using TrucksLogisticsClient.Models;
using TrucksLogisticsClient.Models.Helping_Models;

namespace TrucksLogisticsClient.Pages;

[QueryProperty(nameof(UserID), "UserID")]
public partial class UserMainMenuPage : ContentPage
{
	public int UserID { get; set; }

	public HttpClient client = new HttpClient();

	private string apiUrl;

	private List<Language> SelectedLanguages = new List<Language>();

	private PaginationPage pages = new PaginationPage();

	private int totalassignedjobs;

	private int totalopenjobs;

	private string jobssectionfilter;

    public Users? CurrentUser { get; set; }
    public UserMainMenuPage()
	{
		InitializeComponent();

	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var token = await SecureStorage.GetAsync("auth_token");

        if (token != null)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        apiUrl = Preferences.Get("api_url", "127.0.0.1:5160/api/");

        bool GotUser = await GetUser();

        if (GotUser)
        {
            this.BindingContext = CurrentUser;
        }

    }

    private async Task HideEverything()
	{
		Overview_Section.IsVisible = false;
		Overview_Section.IsEnabled = false;

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
	private async Task<bool> GetUser()
	{
		var response = await client.GetAsync(apiUrl + "Users/Get_User_By_ID/" + UserID);

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

    private async Task<List<Job>> GetPageJobs(string jobssectionfilter)
    {

		if(jobssectionfilter == "open")
		{
            var response = await client.GetAsync(apiUrl + $"Jobs/Get_Open_Jobs_Page/{pages.PageNumber}/{pages.PageSize}");

			if(response.IsSuccessStatusCode)
			{
                var joblistpage = await response.Content.ReadFromJsonAsync<List<Job>>();
                if (joblistpage.Count == 0)
                {
                    return new List<Job>();
                }
                return joblistpage;
            }
        }
		else
		{
            var response = await client.GetAsync(apiUrl + $"Jobs/Get_Assigned_Jobs_Page/{pages.PageNumber}/{pages.PageSize}/{UserID}");

			if(response.IsSuccessStatusCode)
			{
                var joblistpage = await response.Content.ReadFromJsonAsync<List<Job>>();
                if (joblistpage.Count == 0)
                {
                    return new List<Job>();
                }
                return joblistpage;
            }
        }
        return new List<Job>();

    }

	private async Task GetJobTotalCount()
	{
		var response = await client.GetAsync(apiUrl + $"Jobs/Get_Jobs_Stats_User/{CurrentUser.ID}");

		if(response.IsSuccessStatusCode)
		{
			var stats = await response.Content.ReadFromJsonAsync<JobStats>();

			totalassignedjobs = stats.Assigned_Count;
			totalopenjobs = stats.Open_Count;
		}

    }

    private async void Right_PageJobs(object sender, EventArgs e)
    {
        if (pages.PageNumber < pages.TotalPages)
        {
            pages.PageNumber++;
            var jobs = await GetPageJobs(jobssectionfilter);
            Jobs_View_Collection.ItemsSource = jobs;
        }
    }

    private async void Left_PageJobs(object sender, EventArgs e)
    {
        if (pages.PageNumber > 1)
        {
            pages.PageNumber--;
            var jobs = await GetPageJobs(jobssectionfilter);
            Jobs_View_Collection.ItemsSource = jobs;
        }
    }

    private async void First_PageJobs(object sender, EventArgs e)
    {
        pages.PageNumber = 1;
        var jobs = await GetPageJobs(jobssectionfilter);
        Jobs_View_Collection.ItemsSource = jobs;
    }

    private async void Last_PageJobs(object sender, EventArgs e)
    {
        pages.PageNumber = pages.TotalPages;
        var jobs = await GetPageJobs(jobssectionfilter);
        Jobs_View_Collection.ItemsSource = jobs;
    }

	private async void User_Show_Overview(object sender, EventArgs e)
	{
		await HideEverything();
		Overview_Section.IsVisible = true;
		Overview_Section.IsEnabled = true;
	}

    private async void User_Show_Data(object sender, EventArgs e)
    {
		await HideEverything();
		this.BindingContext = CurrentUser;
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
            jobssectionfilter = "assigned";

            pages.PageNumber = 1;
			var jobs = await GetPageJobs(jobssectionfilter);

            await GetJobTotalCount();

            pages.TotalPages = (int)Math.Ceiling((double)totalassignedjobs / pages.PageSize);

			Jobs_View_Collection.ItemsSource = jobs;

            Jobs_Page_Label.Text = $"{pages.PageNumber} / {pages.TotalPages}";
        }
	}


	private async void User_Show_Available_Jobs(object sender, EventArgs e)
	{
		await HideEverything();

        Jobs_View.IsVisible = true;
        Jobs_View.IsEnabled = true;

		if (CurrentUser != null)
		{

            jobssectionfilter = "open";

            pages.PageNumber = 1;

			await GetJobTotalCount();

            pages.TotalPages = (int)Math.Ceiling((double)totalopenjobs / pages.PageSize);
            
			var jobs = await GetPageJobs(jobssectionfilter);

            Jobs_View_Collection.ItemsSource = jobs;

            Jobs_Page_Label.Text = $"{pages.PageNumber} / {pages.TotalPages}";
        }
    }

	private async void User_Jobs_View_Selected(object sender, SelectionChangedEventArgs e)
	{
		
		var selectedjob = e.CurrentSelection.FirstOrDefault() as Job;

		if(selectedjob == null)
		{
			return;
		}

        await HideEverything();
        User_Show_Chosen_Job.IsVisible = true;
        User_Show_Chosen_Job.IsEnabled = true;
        User_Show_Chosen_Job.BindingContext = selectedjob;

		//check if met requierments for job
		if (selectedjob.Status == "open" && CurrentUser != null)
		{
            ApplyForJobSection.IsVisible = true;
            ApplyForJobSection.IsEnabled = true;
            CancelJobSection.IsVisible = false;
            CancelJobSection.IsEnabled = false;

            var usertrucks = CurrentUser.AssignedTrucks.ToList();
			var UserLanguagesNames = CurrentUser.Languages.Select(x => x.Name).ToList();
			var requiredlanguages = selectedjob.RequiredLanguages.Split(",").ToList();

			bool MetTruckRequierments = usertrucks.Any(x => x.Capacity >= selectedjob.RequiredMinimumCapacity &&
			(x.brand == selectedjob.RequiredTruckBrand || selectedjob.RequiredTruckBrand == "all"));

			bool MetLanguageRequierments = UserLanguagesNames.All(x => UserLanguagesNames.Contains(x));

			if(MetLanguageRequierments && MetTruckRequierments)
			{
				ApplyForJobLabel.Text = "You met all requierments of this job,\nclick to apply.";
				ApplyForJobButton.IsEnabled = true;
				Debug.WriteLine("Met all requierments.");
			}
			else
			{
                ApplyForJobLabel.Text = "You don't meet the requierments of this job,\nyou can't apply.";
				ApplyForJobButton.IsEnabled = false;
                Debug.WriteLine("Didnt met requierments.");
			}

		}
		if (selectedjob.Status == "assigned" && CurrentUser != null)
		{
			ApplyForJobSection.IsVisible = false;
			ApplyForJobSection.IsEnabled = false;
			CancelJobSection.IsVisible = true;
			CancelJobSection.IsEnabled = true;

			CancelJobLabel.Text = "Click to cancel assigment for this job.";
		}

    }

	private async void Show_User_Edit(object sender, EventArgs e)
	{
		await HideEverything();



        if (CurrentUser == null)
		{
			return;
		}

		var response = await client.GetAsync(apiUrl + "Values/Get_Languages");

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
            All_Languages_View.ItemsSource = AllLanguages;
        }

        Edit_User_Section.BindingContext = CurrentUser;

		Edit_User_Section.IsVisible = true;
		Edit_User_Section.IsEnabled = true;
	}

	private async void Save_User_Edit(object sender, EventArgs e)
	{
		var selecteduser = Edit_User_Section.BindingContext as Users;

		var Languages = SelectedLanguages;

        #region usercheck

		if(string.IsNullOrEmpty(selecteduser.FirstName))
		{
			EditUserLabelMain.Text = "FirstName cannot be null.";
			return;
		}
        if (string.IsNullOrEmpty(selecteduser.LastName))
        {
            EditUserLabelMain.Text = "LastName cannot be null.";
            return;
        }
        if (!int.TryParse(selecteduser.Age.ToString(), out int age))
        {
            EditUserLabelMain.Text = "Age has to be a number.";
            return;
        }
        if (string.IsNullOrEmpty(selecteduser.Username))
        {
            EditUserLabelMain.Text = "Username cannot be null.";
            return;
        }
        if (!string.IsNullOrEmpty(Edit_User_New_Password.Text))
        {
            selecteduser.Password = Edit_User_New_Password.Text;
        }

        #endregion

        if (selecteduser != null && Languages != null)
		{
			var response = await client.PutAsJsonAsync(apiUrl + "Users/Update_User/" + selecteduser.ID, selecteduser);

			var response2 = await client.PutAsJsonAsync(apiUrl + "Users/Update_User_Languages/" + selecteduser.ID, Languages);

			if(response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
			{
				EditUserLabelMain.Text = await response.Content.ReadAsStringAsync() + "\n" + await response2.Content.ReadAsStringAsync();

				CurrentUser = selecteduser;
				this.BindingContext = null;
				this.BindingContext = CurrentUser;
			}
			else
			{
				EditUserLabelMain.Text = await response.Content.ReadAsStringAsync() + "\n" + await response2.Content.ReadAsStringAsync();
            }
		}
		
	}

	private async void On_Language_Tapped(object sender, EventArgs e)
	{
		var border = (Border)sender;

		var language = (Language)border.BindingContext; 

		if (language != null)
		{
			if(SelectedLanguages.Contains(language))
			{
                SelectedLanguages.Remove(language);
                language.SelectionColor = Colors.Transparent;
				
			}
            else if(!SelectedLanguages.Contains(language))
            {
                SelectedLanguages.Add(language);
                language.SelectionColor = Colors.LightBlue;
				
            }
        }
	}

	private async void Apply_For_Job(object sender, EventArgs e)
	{
		var chosenjob = User_Show_Chosen_Job.BindingContext as Job;

		if(chosenjob != null && CurrentUser != null)
		{
			chosenjob.Status = "assigned";
			chosenjob.AssignedUserId = CurrentUser.ID;

			var response = await client.PutAsJsonAsync(apiUrl + "Jobs/Update_Job/" + chosenjob.ID, chosenjob);

			if(response.IsSuccessStatusCode)
			{
				ApplyForJobLabel.Text = "Successfully applied for job\n\n" + await response.Content.ReadAsStringAsync();
				ApplyForJobButton.IsEnabled = false;

				//reset current user for new job
				await GetUser();
			}
			else
			{
				ApplyForJobLabel.Text = await response.Content.ReadAsStringAsync();
			}
		}
	}

	private async void Cancel_Job(object sender, EventArgs e)
	{
		var chosenjob = User_Show_Chosen_Job.BindingContext as Job;

		if (chosenjob != null && CurrentUser != null)
		{
			chosenjob.AssignedUserId = null;

			chosenjob.Status = "open";

			var response = await client.PutAsJsonAsync(apiUrl + "Jobs/Update_Job/" + chosenjob.ID, chosenjob);

			if(response.IsSuccessStatusCode)
			{
				CancelJobLabel.Text = "Successfully canceled assignment for this job. \n\n" + await response.Content.ReadAsStringAsync();
				CancelJobButton.IsEnabled = false;

				//reset userdata
				await GetUser();
			}
			else
			{
				CancelJobLabel.Text = await response.Content.ReadAsStringAsync();
            }
		}
	}

	private async void Log_Out(object sender, EventArgs e)
	{
		SecureStorage.Remove("auth_token");

		await Shell.Current.GoToAsync("//MainPage");
	}
	
}