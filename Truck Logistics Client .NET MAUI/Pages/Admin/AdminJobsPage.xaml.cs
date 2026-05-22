using System.Diagnostics;
using System.Net.Http.Json;
using TrucksLogisticsClient.Models;
using TrucksLogisticsClient.Models.Helping_Models;
using TrucksLogisticsServerAPI.Models.Helping_Models;

namespace TrucksLogisticsClient.Pages;

[QueryProperty(nameof(UserID), "UserID")]
public partial class AdminJobsPage : ContentPage
{
    public int UserID { get; set; }

    private bool isUserDataFetched = false;

    public Users? CurrentUser { get; set; }

    private List<Language> SelectedLanguages = new List<Language>();

    private List<Truck> SelectedTrucks = new List<Truck>();

    private string apiUrl;

    private PaginationPage pages = new PaginationPage();

    private HttpClient client = new HttpClient();
    public AdminJobsPage()
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

        if(token != null)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        apiUrl = Preferences.Get("api_url", "127.0.0.1:5160/api/");

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
                }
            }
            this.BindingContext = CurrentUser;

            Admin_Data_Panel.IsEnabled = true;
            Admin_Data_Panel.IsVisible = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error fetching user data: " + ex.Message);
        }


    }

    private async Task Hide_Everything()
    {
        Jobs_View.IsVisible = false;
        Jobs_View.IsEnabled = false;

        Edit_Job_Section.IsVisible = false;
        Edit_Job_Section.IsEnabled = false;

        Add_Job_Section.IsVisible = false;
        Add_Job_Section.IsEnabled = false;


    }

    private async Task<List<Language>> Get_Languages()
    {
        try
        {
            var response = await client.GetAsync(apiUrl + "Values/Get_Languages");

            if (response.IsSuccessStatusCode)
            {
                var Languages = await response.Content.ReadFromJsonAsync<List<Language>>();

                if (Languages != null)
                {
                    return Languages;
                }
                else
                {
                    return new List<Language>();
                }
            }
        }
        catch (Exception ex)
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
            var response = await client.GetAsync(apiUrl + "Clients/Get_Clients");

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

    //Pagination

    private async Task<List<Job>> GetPageJobs()
    {

        var response = await client.GetAsync(apiUrl + $"Jobs/Get_Jobs_Page/{pages.PageNumber}/{pages.PageSize}");

        if (response.IsSuccessStatusCode)
        {
            var joblistpage = await response.Content.ReadFromJsonAsync<List<Job>>();
            if (joblistpage.Count == 0)
            {
                return new List<Job>();
            }
            return joblistpage;
        }

        return new List<Job>();

    }

    private async Task<int> CountTotalPagesJobs()
    {
        var response = await client.GetAsync(apiUrl + "Jobs/Get_Jobs_Stats");

        if (response.IsSuccessStatusCode)
        {
            var stats = await response.Content.ReadFromJsonAsync<JobStats>();

            if (stats == null)
            {
                return 0;
            }

            var totalpages = (int)Math.Ceiling((double)stats.Jobs_Count / pages.PageSize);

            return totalpages;
        }

        return 0;
    }

    //GET USERS, TRUCKS, JOBS

    private async void Admin_Get_Jobs_Clicked(object sender, EventArgs e)
    {
        await Hide_Everything();

        Jobs_View.IsEnabled = true;
        Jobs_View.IsVisible = true;

        pages.PageNumber = 1;

        try
        {
            pages.TotalPages = await CountTotalPagesJobs();

            var jobs = await GetPageJobs();

            Get_All_Jobs_View.ItemsSource = jobs;

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
            return;
        }
    }


    //OPEN CERTAIN SECTIONS IN MAIN MENU

    private async void Admin_Open_Add_Job_Section(object sender, EventArgs e)
    {
        await Hide_Everything();
        Add_Job_Section.IsVisible = true;
        Add_Job_Section.IsEnabled = true;

        SelectedLanguages.Clear();

        var languagesfromdb = await Get_Languages();

        Admin_Add_Job_RequiredLanguages_View.ItemsSource = languagesfromdb;

        var allclients = await Get_Clients();

        Admin_Add_Job_Clients_View.ItemsSource = allclients;
    }

    private async void Admin_Jobs_View_Selected(object sender, SelectionChangedEventArgs e)
    {
        await Hide_Everything();
        var selectedJob = e.CurrentSelection.FirstOrDefault() as Job;
        if (selectedJob != null)
        {
            Edit_Job_Section_Header.Text = "Edit job " + selectedJob.Name;
            Edit_Job_Section.IsEnabled = true;
            Edit_Job_Section.IsVisible = true;
            Edit_Job_Section.BindingContext = selectedJob;
        }

        SelectedLanguages.Clear();
        var alllanguages = await Get_Languages();

        var selectedlanguagesstring = selectedJob.RequiredLanguages.Split(",");

        foreach (var lang in alllanguages)
        {
            if (selectedlanguagesstring.Contains(lang.Name))
            {
                lang.SelectionColor = Colors.LightBlue;
                SelectedLanguages.Add(lang);
            }
            else
            {
                lang.SelectionColor = Colors.Transparent;
            }
        }



        try
        {
            var response = await client.GetAsync(apiUrl + "Users/Get_All_Users");

            if (response.IsSuccessStatusCode)
            {
                var userslist = await response.Content.ReadFromJsonAsync<List<Users>>();

                Admin_Edit_Job_Users_View.ItemsSource = userslist;

                var assigneduser = userslist.FirstOrDefault(x => x.ID == selectedJob.AssignedUserId);

                if (assigneduser != null)
                {
                    Admin_Edit_Job_Users_View.SelectedItem = assigneduser;
                }
                else
                {
                    Admin_Edit_Job_Users_View.SelectedItem = null;
                }

            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
            return;
        }

        Admin_Edit_Job_RequiredLanguages_View.ItemsSource = alllanguages;
    }

    //ADD TO DATABASE

    private async void Admin_Add_Job_Clicked(object sender, EventArgs e)
    {
        Job JobToAdd = new Job();

        if (string.IsNullOrEmpty(Admin_Add_Job_Name.Text))
        {
            Add_Job_Error_Label.Text = "Job Name is empty!";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_Job_Description.Text))
        {
            Add_Job_Error_Label.Text = "Job Description is empty!";
            return;
        }
        if (!int.TryParse(Admin_Add_Job_RequiredMinimumCapacity.Text, out int minimumCapacity))
        {
            Add_Job_Error_Label.Text = "Minimum Capacity should be a number!";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_Job_LocationFrom.Text) || string.IsNullOrEmpty(Admin_Add_Job_LocationTo.Text))
        {
            Add_Job_Error_Label.Text = "Location From and Location To cannot be empty!";
            return;
        }
        if (Admin_Add_Job_RequiredTruckBrand.Text == null)
        {
            JobToAdd.RequiredTruckBrand = "all";
        }
        else
        {
            JobToAdd.RequiredTruckBrand = Admin_Add_Job_RequiredTruckBrand.Text;
        }
        if (SelectedLanguages.Count == 0)
        {
            Add_Job_Error_Label.Text = "Select at least one required language!";
            return;
        }
        if (Admin_Add_Job_ClientContact.Text == null)
        {
            Add_Job_Error_Label.Text = "Add Client contact number!";
            return;
        }
        if (Admin_Add_Job_CompanyName.Text == null)
        {
            Add_Job_Error_Label.Text = "Add Client Company Name!";
            return;
        }
        if (Admin_Add_Job_Clients_View.SelectedItem == null)
        {
            Add_Job_Error_Label.Text = "Choose client.";
            return;
        }
        // get selected languages and convert to string separated by comma

        var selectedlanguagesstring = string.Join(",", SelectedLanguages.Select(x => x.Name));


        JobToAdd.Name = Admin_Add_Job_Name.Text;
        JobToAdd.CompanyName = Admin_Add_Job_CompanyName.Text;
        JobToAdd.ClientContactNumber = Admin_Add_Job_ClientContact.Text;
        JobToAdd.Created = DateTime.Now;
        JobToAdd.DeadLine = (DateTime)Admin_Add_Job_DeadLine.Date;
        JobToAdd.LocationFrom = Admin_Add_Job_LocationFrom.Text;
        JobToAdd.LocationTo = Admin_Add_Job_LocationTo.Text;
        JobToAdd.Status = "open";
        JobToAdd.Description = Admin_Add_Job_Description.Text;


        JobToAdd.RequiredLanguages = selectedlanguagesstring;
        JobToAdd.RequiredMinimumCapacity = minimumCapacity;

        Client ClientToAdd = (Client)Admin_Add_Job_Clients_View.SelectedItem;

        JobToAdd.ClientID = ClientToAdd.ID;

        var response = await client.PostAsJsonAsync(apiUrl + "Jobs/Add_Job", JobToAdd);

        if (response.IsSuccessStatusCode)
        {
            Add_Job_Error_Label.Text = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(await response.Content.ReadAsStringAsync());
        }
        else
        {
            Add_Job_Error_Label.Text = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }

    //SAVE EDIT TO DATABASE

    private async void Admin_Save_Job_Edit(object sender, EventArgs e)
    {
        var selectedjob = Edit_Job_Section.BindingContext as Job;

        if (selectedjob != null)
        {

            var selectedlanguagesstring = string.Join(",", SelectedLanguages.Select(x => x.Name));

            selectedjob.RequiredLanguages = selectedlanguagesstring;

            var selecteduser = Admin_Edit_Job_Users_View.SelectedItem as Users;
            if (selecteduser != null)
            {
                selectedjob.AssignedUserId = selecteduser.ID;
            }
            else
            {
                selectedjob.AssignedUserId = null;
            }


            var response = await client.PutAsJsonAsync(apiUrl + "Jobs/Update_Job/" + selectedjob.ID, selectedjob);


            if (response.IsSuccessStatusCode)
            {
                Edit_Job_Error_Label.Text = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Truck updated successfully.");
            }
            else
            {
                Edit_Job_Error_Label.Text = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Failed to update Job, status code: " + response.Content.ReadAsStringAsync());
            }

            return;
        }
        else
        {
            //dalej nie wiem co trzeba zrobic aby to osiagnac
            Edit_Job_Error_Label.Text = "No truck selected for editing.";
            Debug.WriteLine("No job selected for editing.");
            return;
        }

    }

    //DELETE FROM DATABASE


    private async void Admin_Delete_Job(object sender, EventArgs e)
    {
        var JobToDelete = Edit_Job_Section.BindingContext as Job;

        if (JobToDelete != null)
        {
            var response = await DisplayAlertAsync("Deleting Job", "Are you sure you want to delete Job: " + JobToDelete.Name + "?", "Yes", "No");

            if (response)
            {
                var request = await client.DeleteAsync(apiUrl + "Jobs/Delete_Job/" + JobToDelete.ID);
                if (request.IsSuccessStatusCode)
                {
                    Debug.Write("Deleted Job from Database.");
                    Edit_Job_Error_Label.Text = await request.Content.ReadAsStringAsync();
                    await Hide_Everything();
                    return;
                }

                Edit_Job_Error_Label.Text = await request.Content.ReadAsStringAsync();
            }

            return;

        }
    }

    //other stuff

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

    private async void Clear_Assign_Clicked(object sender, EventArgs e)
    {
        Admin_Edit_Job_Users_View.SelectedItem = null;
    }

    private async void Admin_Go_Back(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}