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

    private string apiUrl = "http://192.168.0.218:5160/api/Values/";

    private HttpClient client = new HttpClient();

    public MainMenuPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Get_Current_User();
    }

    //GET CURRENT USER, HIDE EVERYTHING, GET LANGUAGES
    #region UserManagement
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

        Jobs_View.IsVisible = false;
        Jobs_View.IsEnabled = false;

        Edit_User_Section.IsVisible = false;
        Edit_User_Section.IsEnabled = false;

        Edit_Truck_Section.IsVisible = false;
        Edit_Truck_Section.IsEnabled = false;

        Add_User_Section.IsVisible = false;
        Add_User_Section.IsEnabled = false;

        Add_Truck_Section.IsVisible = false;
        Add_Truck_Section.IsEnabled = false;

        
    }

    private async Task<List<Language>> Get_Languages()
    {
        try
        {
            var response = await client.GetAsync(apiUrl + "Get_Languages");
    
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

    #endregion


    //GET USERS, TRUCKS, JOBS
    #region GetData
    private async void Admin_Get_Users_Clicked(object sender, EventArgs e)
    {
        await Hide_Everything();
        try
        {
            var response = await client.GetAsync(apiUrl + "Get_All_Users");

            if(response.IsSuccessStatusCode)
            {
                var userslist = await response.Content.ReadFromJsonAsync<List<Users>>();

                Get_All_Users_View.ItemsSource = userslist;

                
            }
        }
        catch(Exception ex)
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
            var response = await client.GetAsync(apiUrl + "Get_Trucks");
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

    private async void Admin_Get_Jobs_Clicked(object sender, EventArgs e)
    {
        await Hide_Everything();

        Jobs_View.IsEnabled = true;
        Jobs_View.IsVisible = true;

        try
        {
            var response = await client.GetAsync(apiUrl + "Get_Jobs");
            if (response.IsSuccessStatusCode)
            {
                var jobslist = await response.Content.ReadFromJsonAsync<List<Job>>();
                Get_All_Jobs_View.ItemsSource = jobslist;
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
            return;
        }
    }

    #endregion

    //OPEN CERTAIN SECTIONS IN MAIN MENU
    #region OpenSections
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

    private async void Admin_Open_Add_Job_Section(object sender, EventArgs e)
    {
        await Hide_Everything();
        //Add_Job_Section.IsVisible = true;
        //Add_Job_Section.IsEnabled = true;
    }

    private async void Admin_Users_View_Selected(object sender, SelectionChangedEventArgs e)
    {
        await Hide_Everything();
        

        var selecteduser = e.CurrentSelection.FirstOrDefault() as Users;

        // get all languages
        var allLanguages = await Get_Languages();

        var allTrucks = await client.GetFromJsonAsync<List<Truck>>(apiUrl + "Get_Trucks");

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
            foreach(var lang in allLanguages)
            {
                if(selecteduser.Languages.Any(x => x.Id == lang.Id))
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

        var selectedTruck = e.CurrentSelection.FirstOrDefault() as Truck;

        if(selectedTruck != null)
        {
            EditTruckLabelHeader.Text = "Edit truck " + selectedTruck.Name;
            Edit_Truck_Section.IsEnabled = true;
            Edit_Truck_Section.IsVisible = true;

            Edit_Truck_Section.BindingContext = selectedTruck;
        }
    }

    private async void Admin_Jobs_View_Selected(object sender, SelectionChangedEventArgs e)
    {
        await Hide_Everything();
        var selectedJob = e.CurrentSelection.FirstOrDefault() as Job;
        if(selectedJob != null)
        {
            //EditJobLabelHeader.Text = "Edit job " + selectedJob.Name;
            //Edit_Job_Section.IsEnabled = true;
            //Edit_Job_Section.IsVisible = true;
            //Edit_Job_Section.BindingContext = selectedJob;
        }
    }

    #endregion

    //ADD TO DATABASE
    #region AddToDatabase
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
            Add_User_Error_Label.Text = "Age is just a number!";
            return;
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

        var result = await client.PostAsJsonAsync(apiUrl + "Add_User", UserToAdd);

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

        var response = await client.PostAsJsonAsync(apiUrl + "Add_Truck", TruckToAdd);

        if (response.IsSuccessStatusCode)
        {
            Add_Truck_Error_Label.Text = await response.Content.ReadAsStringAsync();
        }
        else
        {
            Add_Truck_Error_Label.Text = await response.Content.ReadAsStringAsync();
        }



    }

    private async void Admin_Add_Job_Clicked(object sender, EventArgs e)
    {

    }
    #endregion

    //SAVE EDIT TO DATABASE
    #region SaveEditToDatabase
    private async void Admin_Save_User_Edit(object sender, EventArgs e)
    {
        var selecteduser = Edit_User_Section.BindingContext as Users;

        //get selected languages
        var selectedlanguages = SelectedLanguages;
        if (selecteduser != null)
        {
            var result = await client.PutAsJsonAsync(apiUrl + "Update_User/" + selecteduser.ID, selecteduser);

            //http put update languages
            var result2 = await client.PutAsJsonAsync(apiUrl + "Update_User_Languages/" + selecteduser.ID, selectedlanguages);

            var result3 = await client.PutAsJsonAsync(apiUrl + "Update_User_Trucks/" + selecteduser.ID, SelectedTrucks);
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
            var result = await client.PutAsJsonAsync(apiUrl + "Update_Truck/" + selectedtruck.Id, selectedtruck);
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

            await Hide_Everything();

            Trucks_View.IsEnabled = true;
            Trucks_View.IsVisible = true;

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

    private async void Admin_Save_Job_Edit(object sender, EventArgs e)
    {

    }

    #endregion

    //DELETE FROM DATABASE

    #region DeleteFromDatabase

    private async void Admin_Delete_User(object sender, EventArgs e)
    {
        var selecteduser = Edit_User_Section.BindingContext as Users;

        if(selecteduser != null)
        {
            var response = await DisplayAlertAsync("Deleting User", "Are you sure you want to delete " + selecteduser.Username, "Yes", "No");

            if(response)
            {
                var request = await client.DeleteAsync(apiUrl + "Delete_User/" + selecteduser.ID);

                if(request.IsSuccessStatusCode)
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

        if(selectedtruck != null)
        {
            var response = await DisplayAlertAsync("Deleting Truck", "Are you sure you want to delete " + selectedtruck.Name, "Yes", "No");

            if(response)
            {
                var request = await client.DeleteAsync(apiUrl + "Delete_Truck/" + selectedtruck.Id);

                if(request.IsSuccessStatusCode)
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

    private async void Admin_Delete_Job(object sender, EventArgs e)
    {

    }

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

    private async void On_Job_Tapped(object sender, EventArgs e)
    {

    }

    #endregion
}