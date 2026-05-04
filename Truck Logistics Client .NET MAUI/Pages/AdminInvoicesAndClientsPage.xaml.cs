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

        Client_View.IsVisible = false;
        Client_View.IsEnabled = false;

        Edit_Client_Section.IsVisible = false;
        Edit_Client_Section.IsEnabled = false;
    }
    // open sections
    private async void Admin_Show_Clients_View(object sender, EventArgs e)
    {
        await HideEverything();

        Client_View.IsVisible = true;
        Client_View.IsEnabled = true;
        
        var clients = await client.GetFromJsonAsync<List<Client>>(apiUrl + "Get_Clients");

        All_Clients_View.ItemsSource = clients;

        Console.WriteLine(clients);
    }

    private async void Admin_Client_View_Selected(object sender, SelectionChangedEventArgs e)
    {
        await HideEverything();

        Edit_Client_Section.IsVisible = true;
        Edit_Client_Section.IsEnabled = true;

        var SelectedClient = e.CurrentSelection.FirstOrDefault() as Client;

        Edit_Client_Section.BindingContext = SelectedClient;
    }

    private async void Admin_Open_Add_Client_Section(object sender, EventArgs e)
    {
        await HideEverything();

        Add_Client_Section.IsVisible = true;
        Add_Client_Section.IsEnabled = true;
    }

    private async void Admin_Save_Client_Edit(object sender, EventArgs e)
    {

    }
    private async void Admin_Delete_Client(object sender, EventArgs e)
    {

    }
    private async void Admin_Open_Add_Invoice_Section(object sender, EventArgs e)
    {

    }

    // add to database 
    private async void Admin_Add_Client_Clicked(object sender, EventArgs e)
    {
        Client ClientToAdd = new Client();

        #region check if ok section

        if(string.IsNullOrEmpty(Admin_Add_Client_Name.Text))
        {
            Add_Client_Error_Label.Text = "Client Name cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_Client_NIP.Text))
        {
            Add_Client_Error_Label.Text = "Client NIP cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_Client_Country.Text))
        {
            Add_Client_Error_Label.Text = "Client Country cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_Client_City.Text))
        {
            Add_Client_Error_Label.Text = "Client City cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_Client_Address.Text))
        {
            Add_Client_Error_Label.Text = "Client Address cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_Client_PostalCode.Text))
        {
            Add_Client_Error_Label.Text = "Client PostalCode cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_Client_Phone.Text))
        {
            Add_Client_Error_Label.Text = "Client Phone cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Add_Client_Email.Text))
        {
            Add_Client_Error_Label.Text = "Client Email cant be null.";
            return;
        }
        #endregion



        ClientToAdd.Name = Admin_Add_Client_Name.Text;
        ClientToAdd.NIP = Admin_Add_Client_NIP.Text;
        ClientToAdd.Country = Admin_Add_Client_Country.Text;
        ClientToAdd.City = Admin_Add_Client_City.Text;
        ClientToAdd.Address = Admin_Add_Client_Address.Text;
        ClientToAdd.PostalCode = Admin_Add_Client_PostalCode.Text;
        ClientToAdd.Phone = Admin_Add_Client_Phone.Text;
        ClientToAdd.Email = Admin_Add_Client_Email.Text;

        var response = await client.PostAsJsonAsync(apiUrl + "Add_Client", ClientToAdd);

        if (response.IsSuccessStatusCode)
        {
            Add_Client_Error_Label.Text = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("Successfully added client.");
        }
        else
        {
            Add_Client_Error_Label.Text = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("Error Adding client.");
        }

    }
    private async void Admin_Go_Back(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    
}