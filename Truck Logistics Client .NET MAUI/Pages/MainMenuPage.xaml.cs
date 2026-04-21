namespace TrucksLogisticsClient.Pages;

using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrucksLogisticsClient.Models;

[QueryProperty(nameof(UserID), "UserID")]
public partial class MainMenuPage : ContentPage
{
    public int UserID { get; set; }

    private bool isUserDataFetched = false;
    public Users CurrentUser { get; set; }

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
        if(isUserDataFetched)
        {
            this.BindingContext = CurrentUser;
            await Generate_Main_Menu();
        }
        

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
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error fetching user data: " + ex.Message);
        }
        

    }

    private async Task Generate_Main_Menu()
    {
        if(CurrentUser.Role == "admin")
        {
            User_Get_Trucks.IsEnabled = false;
            User_Get_Trucks.IsVisible = false;

            Admin_Data_Panel.IsEnabled = true;
            Admin_Data_Panel.IsVisible = true;
            
        }
        if (CurrentUser.Role == "user")
        {
            User_Get_Trucks.IsEnabled = true;
            User_Get_Trucks.IsVisible = true;

            Admin_Data_Panel.IsEnabled = false;
            Admin_Data_Panel.IsVisible = false;

        }
    }

    private async Task Hide_Everything()
    {
        Users_View.IsVisible = false;
        Users_View.IsEnabled = false;

        Trucks_View.IsVisible = false;
        Trucks_View.IsVisible = false;

        Edit_User_Section.IsVisible = false;
        Edit_User_Section.IsEnabled = false;

        Add_User_Section.IsVisible = false;
        Add_User_Section.IsEnabled = false;
    }

    private async void Admin_Get_Users_Clicked(object sender, EventArgs e)
    {
        await Hide_Everything();
        try
        {
            var response = await client.GetAsync(apiUrl + "Get_All_Users");

            if(response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var userslist = JsonSerializer.Deserialize<List<Users>>(json, options);

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
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var truckslist = JsonSerializer.Deserialize<List<Truck>>(json, options);
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

    private async void Admin_Users_View_Selected(object sender, SelectionChangedEventArgs e)
    {
        await Hide_Everything();
        var selecteduser = e.CurrentSelection.FirstOrDefault() as Users;

        if(selecteduser != null)
        {
            EditUserLabelHeader.Text = "Edit user " + selecteduser.Username;
            Edit_User_Section.IsEnabled = true;
            Edit_User_Section.IsVisible = true;

            Edit_User_Section.BindingContext = selecteduser;
        }   
    }

    private async void Admin_Save_Edit(object sender, EventArgs e)
    {
        var selecteduser = Edit_User_Section.BindingContext as Users;
        if (selecteduser != null)
        {
            var result = await client.PutAsJsonAsync(apiUrl + "Update_User/" + selecteduser.ID, selecteduser);

            if(result.IsSuccessStatusCode)
            {
                Debug.WriteLine("User updated successfully.");
            }
            else
            {
                Debug.WriteLine("Failed to update user. Status code: " + result.StatusCode);
            }

            await Hide_Everything();
            return;
        }
        else
        {
            await Hide_Everything();
            Debug.WriteLine("No user selected for editing.");
            return;
        }
    }

    private async void Admin_Add_User_Clicked(object sender, EventArgs e)
    {

    }

    private async void Admin_Open_Add_User_Section(object sender, EventArgs e)
    {
        await Hide_Everything();
        Add_User_Section.IsEnabled = true;
        Add_User_Section.IsVisible = true;
    }
}