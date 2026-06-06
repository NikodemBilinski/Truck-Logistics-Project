using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.InteropServices.ObjectiveC;
using TrucksLogisticsClient.Models;
using TrucksLogisticsClient.Models.Helping_Models;
using TrucksLogisticsServerAPI.Models.Helping_Models;


namespace TrucksLogisticsClient.Pages;

[QueryProperty(nameof(UserID), "UserID")]
public partial class AdminUsersAndTrucksPage : ContentPage
{
	public int UserID { get; set; }

    private bool isUserDataFetched = false;

    public Users? CurrentUser { get; set; }

	private List<Language> SelectedLanguages = new List<Language>();

	private List<Truck> SelectedTrucks = new List<Truck>();

    private string apiUrl;

	private HttpClient client = new HttpClient();

    private PaginationPage pages = new PaginationPage();
	public AdminUsersAndTrucksPage()
	{
		InitializeComponent();

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsVisible = false,
            IsEnabled = false
        });
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

                    //Welcome_User_Label.Text = CurrentUser.Username;
                }
            }
            this.BindingContext = CurrentUser;

            Admin_Data_Panel.IsEnabled = true;
            Admin_Data_Panel.IsVisible = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error fetching user data: " + ex.Message);
            //Welcome_User_Label.Text = "Error fetching user data: " + ex.Message;
        }


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

    private async Task Hide_Everything()
    {
        Overview_Section.IsVisible = false;
        Overview_Section.IsEnabled = false;

        Users_View.IsVisible = false;
        Users_View.IsEnabled = false;

        Trucks_View.IsVisible = false;
        Trucks_View.IsEnabled = false;

        Edit_User_Section.IsVisible = false;
        Edit_User_Section.IsEnabled = false;

        Edit_Truck_Section.IsVisible = false;
        Edit_Truck_Section.IsEnabled = false;

        Add_User_Section.IsVisible = false;
        Add_User_Section.IsEnabled = false;

        Add_Truck_Section.IsVisible = false;
        Add_Truck_Section.IsEnabled = false;


    }

    private async Task Get_Overview_Stats()
    {
        //users
        var response = await client.GetAsync(apiUrl + "Users/Get_Users_Stats");

        if(response.IsSuccessStatusCode)
        {
            var stats = await response.Content.ReadFromJsonAsync<UsersStats>();
            if (stats != null)
            {
                Total_Users_Count.Text = stats.Users_Count.ToString();
                Avaiable_Users_Count.Text = stats.AvaiableUsers_Count.ToString();
                Busy_Users_Count.Text = stats.BusyUsers_Count.ToString();
                Admin_Users_Count.Text = stats.Admin_Count.ToString();
                User_Users_Count.Text = stats.User_Count.ToString();
            }
        }
    }

    #region Pagination
    //users
    private async Task<List<Users>> GetPageUsers()
    {
        
        var response = await client.GetAsync(apiUrl + $"Users/Get_Users_Page/{pages.PageNumber}/{pages.PageSize}");

        Users_Page_Label.Text = $"{pages.PageNumber} / {pages.TotalPages}";

        if (response.IsSuccessStatusCode)
        {
            var userslistpage = await response.Content.ReadFromJsonAsync<List<Users>>();
            if (userslistpage.Count == 0)
            {
                return new List<Users>();
            }
            return userslistpage;
        }

        return new List<Users>();
    
    }

    private async Task<int> CountTotalPagesUsers()
    {
        var response = await client.GetAsync(apiUrl + "Users/Get_Users_Stats");

        if(response.IsSuccessStatusCode)
        {
            var stats = await response.Content.ReadFromJsonAsync<UsersStats>();

            if (stats == null)
            {
                return 0;
            }

            var totalpages = (int)Math.Ceiling((double)stats.Users_Count / pages.PageSize);

            return totalpages;
        }

        return 0;
    }

    private async void Right_PageUsers(object sender, EventArgs e)
    {
        if(pages.PageNumber >= pages.TotalPages)
        {
            return;
        }
        pages.PageNumber++;
        var users = await GetPageUsers();
        Get_All_Users_View.ItemsSource = users;
    }
    private async void Left_PageUsers(object sender, EventArgs e)
    {
        if (pages.PageNumber <= 1)
        {
            return;
        }
        pages.PageNumber--;
        var users = await GetPageUsers();
        Get_All_Users_View.ItemsSource = users;
    }
    private async void First_PageUsers(object sender, EventArgs e)
    {
        pages.PageNumber = 1;
        var users = await GetPageUsers();
        Get_All_Users_View.ItemsSource = users;
    }
    private async void Last_PageUsers(object sender, EventArgs e)
    {
        pages.PageNumber = pages.TotalPages;
        var users = await GetPageUsers();
        Get_All_Users_View.ItemsSource = users;
    }

    // trucks
    private async Task<List<Truck>> GetPageTrucks()
    {

        var response = await client.GetAsync(apiUrl + $"Trucks/Get_Trucks_Page/{pages.PageNumber}/{pages.PageSize}");

        Trucks_Page_Label.Text = $"{pages.PageNumber} / {pages.TotalPages}";

        if (response.IsSuccessStatusCode)
        {
            var truckslistpage = await response.Content.ReadFromJsonAsync<List<Truck>>();
            if (truckslistpage.Count == 0)
            {
                return new List<Truck>();
            }
            return truckslistpage;
        }
        
        return new List<Truck>();


    }

    private async Task<int> CountTotalPagesTrucks()
    {
        var response = await client.GetAsync(apiUrl + "Trucks/Get_Trucks_Stats");
        if (response.IsSuccessStatusCode)
        {
            var stats = await response.Content.ReadFromJsonAsync<TruckStats>();
            if (stats == null)
            {
                return 0;
            }
            var totalpages = (int)Math.Ceiling((double)stats.Truck_Count / pages.PageSize);
            return totalpages;
        }
        return 0;
    }

    private async void Right_PageTrucks(object sender, EventArgs e)
    {
        if (pages.PageNumber >= pages.TotalPages)
        {
            return;
        }
        pages.PageNumber++;
        var trucks = await GetPageTrucks();
        Get_All_Trucks_View.ItemsSource = trucks;
    }
    private async void Left_PageTrucks(object sender, EventArgs e)
    {
        if (pages.PageNumber <= 1)
        {
            return;
        }
        pages.PageNumber--;
        var trucks = await GetPageTrucks();
        Get_All_Trucks_View.ItemsSource = trucks;
    }
    private async void First_PageTrucks(object sender, EventArgs e)
    {
        pages.PageNumber = 1;
        var trucks = await GetPageTrucks();
        Get_All_Trucks_View.ItemsSource = trucks;
    }
    private async void Last_PageTrucks(object sender, EventArgs e)
    {
        pages.PageNumber = pages.TotalPages;
        var trucks = await GetPageTrucks();
        Get_All_Trucks_View.ItemsSource = trucks;
    }

    #endregion

    // GET Users, Trucks

    private async void Admin_Get_Users_Clicked(object sender, EventArgs e)
    {

        await Hide_Everything();
        try
        {
            pages.TotalPages = await CountTotalPagesUsers();
            pages.PageNumber = 1;
            var users = await GetPageUsers();
            Get_All_Users_View.ItemsSource = users;

            
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
            pages.TotalPages = await CountTotalPagesTrucks();
            pages.PageNumber = 1;
            var trucks = await GetPageTrucks();
            Get_All_Trucks_View.ItemsSource = trucks;
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

    private async void Admin_Show_Users_Trucks_Overview(object sender, EventArgs e)
    {
        await Hide_Everything();
        Overview_Section.IsEnabled = true;
        Overview_Section.IsVisible = true;
    }

    private async void Admin_Users_View_Selected(object sender, SelectionChangedEventArgs e)
    {
        await Hide_Everything();

        EditUserLabelMain.Text = string.Empty;

        var selecteduser = e.CurrentSelection.FirstOrDefault() as Users;

        // get all languages
        var allLanguages = await Get_Languages();

        var allTrucks = await client.GetFromJsonAsync<List<Truck>>(apiUrl + "Trucks/Get_Trucks");

        //clear selected languages and trucks lists if it was used before
        SelectedTrucks.Clear();
        SelectedLanguages.Clear();
        // truck section
        if (allTrucks != null)
        {
            foreach (var truck in allTrucks)
            {
                if (selecteduser.AssignedTrucks.Any(x => x.Id == truck.Id))
                {
                    truck.SelectionColor = Colors.LightBlue;
                    SelectedTrucks.Add(truck);
                }
                else
                {
                    truck.SelectionColor = Colors.Transparent;
                }
            }

            All_Trucks_View.ItemsSource = allTrucks;
        }

        if (allLanguages != null)
        {
            foreach (var lang in allLanguages)
            {
                if (selecteduser.Languages.Any(x => x.Id == lang.Id))
                {
                    lang.SelectionColor = Colors.LightBlue;
                    SelectedLanguages.Add(lang);
                }
                else
                {
                    lang.SelectionColor = Colors.Transparent;
                }
            }
            All_Languages_View.ItemsSource = allLanguages;
        }

        if (selecteduser != null)
        {
            EditUserLabelHeader.Text = "Edit user " + selecteduser.Username;
            Edit_User_Section.IsEnabled = true;
            Edit_User_Section.IsVisible = true;

            Edit_User_Section.BindingContext = selecteduser;
        }
    }

    private async void Admin_Trucks_View_Selected(object sender, SelectionChangedEventArgs e)
    {
        await Hide_Everything();

        EditTruckLabelMain.Text = string.Empty;

        var selectedTruck = e.CurrentSelection.FirstOrDefault() as Truck;

        if (selectedTruck != null)
        {
            EditTruckLabelHeader.Text = "Edit truck " + selectedTruck.Name;
            Edit_Truck_Section.IsEnabled = true;
            Edit_Truck_Section.IsVisible = true;

            Edit_Truck_Section.BindingContext = selectedTruck;
        }
    }

    private async void Admin_Open_Add_User_Section(object sender, EventArgs e)
    {
        await Hide_Everything();
        Add_User_Section.IsEnabled = true;
        Add_User_Section.IsVisible = true;
    }

    private async void Admin_Open_Add_Truck_Section(object sender, EventArgs e)
    {
        await Hide_Everything();

        Add_Truck_Section.IsVisible = true;
        Add_Truck_Section.IsEnabled = true;
    }

    //add users trucks

    private async void Admin_Add_User_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Admin_Add_User_FirstName.Text))
        {
            Add_User_Error_Label.Text = "First Name is empty!";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_User_LastName.Text))
        {
            Add_User_Error_Label.Text = "Last Name is empty!";
            return;
        }
        if (!int.TryParse(Admin_Add_User_Age.Text, out int age))
        {
            Add_User_Error_Label.Text = "Age should be a number!";
            return;
        }

        if(Admin_Add_User_Role.Text == null)
        {
            Admin_Add_User_Role.Text = "user";
        }
        Admin_Add_User_Role.Text = Admin_Add_User_Role.Text.ToLower();

        if (Admin_Add_User_Role.Text != "user" && Admin_Add_User_Role.Text != "admin")
        {
            Add_User_Error_Label.Text = "Role is not either user or admin";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_User_Username.Text))
        {
            Add_User_Error_Label.Text = "Username is empty!";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_User_Password.Text))
        {
            Add_User_Error_Label.Text = "Password is empty!";
            return;
        }
        var UserToAdd = new Users()
        {
            FirstName = Admin_Add_User_FirstName.Text,
            LastName = Admin_Add_User_LastName.Text,
            Age = age,
            Role = Admin_Add_User_Role.Text,
            Username = Admin_Add_User_Username.Text,
            Password = Admin_Add_User_Password.Text
        };

        var result = await client.PostAsJsonAsync(apiUrl + "Users/Add_User", UserToAdd);

        if (result.IsSuccessStatusCode)
        {
            Add_User_Error_Label.Text = await result.Content.ReadAsStringAsync();

        }
        else
        {
            Add_User_Error_Label.Text = await result.Content.ReadAsStringAsync();
        }
    }

    private async void Admin_Add_Truck_Clicked(object sender, EventArgs e)
    {

        Truck TruckToAdd = new Truck();

        if (string.IsNullOrEmpty(Admin_Add_Truck_Name.Text))
        {
            Add_Truck_Error_Label.Text = "Name is empty!";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_Truck_Brand.Text))
        {
            Add_Truck_Error_Label.Text = "Brand is empty!";
            return;
        }
        if (!int.TryParse(Admin_Add_Truck_Capacity.Text, out int capacity))
        {
            Add_Truck_Error_Label.Text = "Capacity should be a number!";
            return;
        }

        TruckToAdd.Name = Admin_Add_Truck_Name.Text;
        TruckToAdd.brand = Admin_Add_Truck_Brand.Text;
        TruckToAdd.Capacity = capacity;

        var response = await client.PostAsJsonAsync(apiUrl + "Trucks/Add_Truck", TruckToAdd);

        if (response.IsSuccessStatusCode)
        {
            Add_Truck_Error_Label.Text = await response.Content.ReadAsStringAsync();
        }
        else
        {
            Add_Truck_Error_Label.Text = await response.Content.ReadAsStringAsync();
        }



    }

    //save edits

    private async void Admin_Save_User_Edit(object sender, EventArgs e)
    {
        var selecteduser = Edit_User_Section.BindingContext as Users;

        //get selected languages
        var selectedlanguages = SelectedLanguages;
        if (selecteduser != null)
        {

            if(!string.IsNullOrEmpty(Edit_User_New_Password.Text))
            {
                selecteduser.Password = Edit_User_New_Password.Text;
            }
            var result = await client.PutAsJsonAsync(apiUrl + "Users/Update_User/" + selecteduser.ID, selecteduser);

            //http put update languages
            var result2 = await client.PutAsJsonAsync(apiUrl + "Users/Update_User_Languages/" + selecteduser.ID, selectedlanguages);

            var result3 = await client.PutAsJsonAsync(apiUrl + "Users/Update_User_Trucks/" + selecteduser.ID, SelectedTrucks);
            if (result.IsSuccessStatusCode && result2.IsSuccessStatusCode && result3.IsSuccessStatusCode)
            {
                Debug.WriteLine("User updated successfully.");
            }
            else
            {
                Debug.WriteLine("Failed to update user. Status code: " + result.Content.ReadAsStringAsync());
                Debug.WriteLine("Failed to update user. Status code: " + result2.Content.ReadAsStringAsync());
                Debug.WriteLine("Failed to update user. Status code: " + result3.Content.ReadAsStringAsync());
                EditUserLabelMain.Text = await result.Content.ReadAsStringAsync() + "\n" + await result2.Content.ReadAsStringAsync()
                    + "\n" + await result3.Content.ReadAsStringAsync();
            }

            await Hide_Everything();

            Users_View.IsEnabled = true;
            Users_View.IsVisible = true;

            return;
        }
        else
        {
            await Hide_Everything();
            Debug.WriteLine("No user selected for editing.");
            return;
        }
    }

    private async void Admin_Save_Truck_Edit(object sender, EventArgs e)
    {
        var selectedtruck = Edit_Truck_Section.BindingContext as Truck;
        if (selectedtruck != null)
        {
            var result = await client.PutAsJsonAsync(apiUrl + "Trucks/Update_Truck/" + selectedtruck.Id, selectedtruck);
            if (result.IsSuccessStatusCode)
            {
                EditTruckLabelMain.Text = await result.Content.ReadAsStringAsync();
                Debug.WriteLine("Truck updated successfully.");
            }
            else
            {
                EditTruckLabelMain.Text = await result.Content.ReadAsStringAsync();
                Debug.WriteLine("Failed to update truck. Status code: " + result.Content.ReadAsStringAsync());
            }

            return;
        }
        else
        {
            //szczerze nie wiem co trzeba by bylo zrobic w tym programie aby osiagnac ten komunikat, ale niech bedzie
            EditTruckLabelMain.Text = "No truck selected for editing.";
            Debug.WriteLine("No truck selected for editing.");
            return;
        }
    }

    //DELETE FROM DATABASE

    private async void Admin_Delete_User(object sender, EventArgs e)
    {
        var selecteduser = Edit_User_Section.BindingContext as Users;

        if (selecteduser != null)
        {
            var response = await DisplayAlertAsync("Deleting User", "Are you sure you want to delete " + selecteduser.Username, "Yes", "No");

            if (response)
            {
                var request = await client.DeleteAsync(apiUrl + "Users/Delete_User/" + selecteduser.ID);

                if (request.IsSuccessStatusCode)
                {
                    //show goooooooooooooooooood
                    EditUserLabelMain.Text = await request.Content.ReadAsStringAsync();
                    return;
                }

                //show error
                EditUserLabelMain.Text = await request.Content.ReadAsStringAsync();

            }
            return;
        }
    }

    private async void Admin_Delete_Truck(object sender, EventArgs e)
    {
        var selectedtruck = Edit_Truck_Section.BindingContext as Truck;

        if (selectedtruck != null)
        {
            var response = await DisplayAlertAsync("Deleting Truck", "Are you sure you want to delete " + selectedtruck.Name, "Yes", "No");

            if (response)
            {
                var request = await client.DeleteAsync(apiUrl + "Trucks/Delete_Truck/" + selectedtruck.Id);

                if (request.IsSuccessStatusCode)
                {
                    //gut
                    EditTruckLabelMain.Text = await request.Content.ReadAsStringAsync();
                    await Hide_Everything();
                    return;
                }

                //error
                EditTruckLabelMain.Text = await request.Content.ReadAsStringAsync();
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

    private async void Admin_Go_Back(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

}