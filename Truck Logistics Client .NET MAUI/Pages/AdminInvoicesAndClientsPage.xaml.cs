using CommunityToolkit.Maui.Storage;
using System.Diagnostics;
using System.Net.Http.Json;
using TrucksLogisticsClient.Models;

namespace TrucksLogisticsClient.Pages;

[QueryProperty(nameof(UserID), "UserID")]
public partial class AdminInvoicesAndClientsPage : ContentPage
{
	public int UserID { get; set; }

    private bool isUserDataFetched;

    private bool isVatOK = false;

    public Users? CurrentUser { get; set; }

    private string apiUrl = "http://192.168.0.218:5160/api/";

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

    private async Task HideEverything()
    {
        Add_Client_Section.IsVisible = false;
        Add_Client_Section.IsEnabled = false;

        Client_View.IsVisible = false;
        Client_View.IsEnabled = false;

        Invoice_View.IsVisible = false;
        Invoice_View.IsEnabled = false;

        Edit_Client_Section.IsVisible = false;
        Edit_Client_Section.IsEnabled = false;

        Edit_Invoice_Section.IsVisible = false;
        Edit_Invoice_Section.IsEnabled = false;

        Add_Invoice_Section.IsVisible = false;
        Add_Invoice_Section.IsEnabled = false;
    }
    // open sections
    private async void Admin_Show_Clients_View(object sender, EventArgs e)
    {
        await HideEverything();

        Client_View.IsVisible = true;
        Client_View.IsEnabled = true;
        
        var clients = await client.GetFromJsonAsync<List<Client>>(apiUrl + "Clients/Get_Clients");

        if (clients != null)
        {
            All_Clients_View.ItemsSource = clients;
        }
        else
        {
            Debug.WriteLine("Error fetching clients.");
            return;
        }
    }

    private async void Admin_Show_Invoices_View(object sender, EventArgs e)
    {
        await HideEverything();
        Invoice_View.IsVisible = true;
        Invoice_View.IsEnabled = true;

        var invoices = await client.GetFromJsonAsync<List<Invoice>>(apiUrl + "Invoices/Get_All_Invoices");

        if(invoices != null)
        {
            All_Invoices_View.ItemsSource = invoices;
        }
        else
        {
            Debug.WriteLine("Error fetching invoices.");
            return;
        }
    }

    private async void Admin_Client_View_Selected(object sender, SelectionChangedEventArgs e)
    {
        await HideEverything();

        Edit_Client_Section.IsVisible = true;
        Edit_Client_Section.IsEnabled = true;

        var SelectedClient = e.CurrentSelection.FirstOrDefault() as Client;

        Edit_Client_Section.BindingContext = SelectedClient;
    }

    private async void Admin_Invoice_View_Selected(object sender, SelectionChangedEventArgs e)
    {
        await HideEverything();

        Edit_Invoice_Section.IsVisible = true;
        Edit_Invoice_Section.IsEnabled = true;

        var SelectedInvoice = e.CurrentSelection.FirstOrDefault() as Invoice;

        Edit_Invoice_Section.BindingContext = SelectedInvoice;
    }

    private async void Admin_Open_Add_Client_Section(object sender, EventArgs e)
    {
        await HideEverything();

        Add_Client_Section.IsVisible = true;
        Add_Client_Section.IsEnabled = true;
    }
    private async void Admin_Open_Add_Invoice_Section(object sender, EventArgs e)
    {
        await HideEverything();

        Add_Invoice_Section.IsVisible = true;
        Add_Invoice_Section.IsEnabled = true;


        var clients = await client.GetFromJsonAsync<List<Client>>(apiUrl + "Clients/Get_Clients");

        if (clients != null)
        {
            Add_Invoice_ClientsView.ItemsSource = clients;
        }

    }


    private async void Admin_Save_Client_Edit(object sender, EventArgs e)
    {
        #region check if ok section

        if (string.IsNullOrEmpty(Admin_Edit_Client_Name.Text))
        {
            Add_Client_Error_Label.Text = "Client Name cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Edit_Client_NIP.Text))
        {
            Add_Client_Error_Label.Text = "Client NIP cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Edit_Client_Country.Text))
        {
            Add_Client_Error_Label.Text = "Client Country cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Edit_Client_City.Text))
        {
            Add_Client_Error_Label.Text = "Client City cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Edit_Client_Address.Text))
        {
            Add_Client_Error_Label.Text = "Client Address cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Edit_Client_PostalCode.Text))
        {
            Add_Client_Error_Label.Text = "Client PostalCode cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Edit_Client_Phone.Text))
        {
            Add_Client_Error_Label.Text = "Client Phone cant be null.";
            return;
        }
        if (string.IsNullOrEmpty(Admin_Edit_Client_Email.Text))
        {
            Add_Client_Error_Label.Text = "Client Email cant be null.";
            return;
        }
        #endregion

        var ClientToChange = Edit_Client_Section.BindingContext as Client;

        if (ClientToChange == null)
        {
            return;
        }

        var response = await client.PutAsJsonAsync(apiUrl + "Clients/Edit_Client/" + ClientToChange.ID, ClientToChange);

        if(response.IsSuccessStatusCode)
        {
            Add_Client_Error_Label.Text = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("Successfully edited client.");
        }
        else
        {
            Add_Client_Error_Label.Text = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("Error editing client.");
        }


    }
    private async void Admin_Delete_Client(object sender, EventArgs e)
    {
        var ClientToDelete = Edit_Client_Section.BindingContext as Client;

        if (ClientToDelete == null)
        {
            return;
        }

        var response = await client.DeleteAsync(apiUrl + "Clients/Delete_Client/" + ClientToDelete.ID);

        if (response.IsSuccessStatusCode)
        {
            Add_Client_Error_Label.Text = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("Successfully deleted client.");
        }
        else
        {
            Add_Client_Error_Label.Text = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("Error deleting client.");
        }
    }

    private async void Admin_Delete_Invoice(object sender, EventArgs e)
    {
        var InvoiceToDelete = Edit_Invoice_Section.BindingContext as Invoice;

        if (InvoiceToDelete != null)
        {
            var response = await client.DeleteAsync(apiUrl + "Invoices/Delete_Invoice/" + InvoiceToDelete.ID);

            if(response.IsSuccessStatusCode)
            {
                EditClientLabelMain.Text = await response.Content.ReadAsStringAsync();
            }
            else
            {
                EditClientLabelMain.Text = await response.Content.ReadAsStringAsync();
            }
        }
    }

    private async void Admin_Save_Invoice(object sender, EventArgs e)
    {
        var InvoiceToChange = Edit_Invoice_Section.BindingContext as Invoice;

        if (InvoiceToChange != null)
        {
            var response = await client.PutAsJsonAsync(apiUrl + "Invoices/Edit_Invoice/" + InvoiceToChange.ID, InvoiceToChange);
            if (response.IsSuccessStatusCode)
            {
                EditClientLabelMain.Text = await response.Content.ReadAsStringAsync();
            }
            else
            {
                EditClientLabelMain.Text = await response.Content.ReadAsStringAsync();
            }
        }
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

        var response = await client.PostAsJsonAsync(apiUrl + "Clients/Add_Client", ClientToAdd);

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

    private async void Admin_Add_Invoice_Clicked(object sender, EventArgs e)
    {
        if(Add_Invoice_ClientsView.SelectedItem == null || Add_Invoice_JobsView.SelectedItem == null)
        {
            Add_Invoice_Error_Label.Text = "Choose Client and job.";
        }

        var SelectedClient = Add_Invoice_ClientsView.SelectedItem as Client;

        var SelectedJob = Add_Invoice_JobsView.SelectedItem as Job;

        Invoice InvoiceToAdd = new Invoice();

        #region check if ok section

        if (SelectedClient == null || SelectedJob == null)
        {
            return;
        }
        if(Admin_Add_Invoice_IssueDate.Date == null)
        {
            Add_Invoice_Error_Label.Text = "Issue Date cant be null.";
            return;
        }
        if(Admin_Add_Invoice_DueDate.Date < Admin_Add_Invoice_IssueDate.Date || Admin_Add_Invoice_DueDate.Date == null)
        {
            Add_Invoice_Error_Label.Text = "Due Date cant be null or earlier than Issue Date.";
            return;
        }
        if(Admin_Add_Invoice_NetAmount.Text == null || !decimal.TryParse(Admin_Add_Invoice_NetAmount.Text, out var amount))
        {
            Add_Invoice_Error_Label.Text = "Net Amount cant be null and must be a number.";
            return;
        }
        if(Admin_Add_Invoice_VatRate.Text == null || !int.TryParse(Admin_Add_Invoice_VatRate.Text, out var vatRate))
        {
            Add_Invoice_Error_Label.Text = "VAT Rate cant be null and must be a number.";
            return;
        }
        if(!isVatOK)
        {
            Add_Invoice_Error_Label.Text = "Check VAT Rate and Net Amount.";
            return;
        }

        #endregion

        InvoiceToAdd.IssueDate = (DateTime)Admin_Add_Invoice_IssueDate.Date;
        InvoiceToAdd.DueDate = (DateTime)Admin_Add_Invoice_DueDate.Date;
        InvoiceToAdd.NetAmount = decimal.Parse(Admin_Add_Invoice_NetAmount.Text);
        InvoiceToAdd.VatRate = int.Parse(Admin_Add_Invoice_VatRate.Text);
        InvoiceToAdd.GrossAmount = decimal.Parse(Admin_Add_Invoice_GrossAmount.Text);
        InvoiceToAdd.ClientID = SelectedClient.ID;
        InvoiceToAdd.JobID = SelectedJob.ID;
        InvoiceToAdd.Status = "unpaid";

        var response = await client.PostAsJsonAsync(apiUrl + "Invoices/Add_Invoice", InvoiceToAdd);

        if(response.IsSuccessStatusCode)
        {
            Add_Invoice_Error_Label.Text = await response.Content.ReadAsStringAsync();
        }
        else
        {
            Add_Invoice_Error_Label.Text = await response.Content.ReadAsStringAsync();
        }
    }

    //other functions
    private async void Admin_Go_Back(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void Vat_Rate_Changed(object sender, EventArgs e)
    {
        if(string.IsNullOrEmpty(Admin_Add_Invoice_VatRate.Text) || string.IsNullOrEmpty(Admin_Add_Invoice_NetAmount.Text))
        {
            isVatOK = false;
            return;
        }

        if(decimal.TryParse(Admin_Add_Invoice_NetAmount.Text, out decimal NetAmount) && int.TryParse(Admin_Add_Invoice_VatRate.Text, out int VatRate))
        {
            decimal GrossAmount = NetAmount + (NetAmount * VatRate / 100);
            Admin_Add_Invoice_GrossAmount.Text = GrossAmount.ToString("F2");
            isVatOK = true;
        }
        else
        {
            Admin_Add_Invoice_GrossAmount.Text = "Invalid input";
            isVatOK = false;
        }
    }

    private async void Add_Invoice_OnClientSelected(object sender, SelectionChangedEventArgs e)
    {
        Add_Invoice_JobsView.SelectedItem = null;
        var SelectedClient = e.CurrentSelection.FirstOrDefault() as Client;

        Add_Invoice_JobsView.IsVisible = true;
        if (SelectedClient != null)
        {
            var response = await client.GetAsync(apiUrl + "Jobs/Get_Jobs_By_Client_ID/" + SelectedClient.ID);

            if(response.IsSuccessStatusCode)
            {
                var ClientJobs = await response.Content.ReadFromJsonAsync<List<Job>>();

                Add_Invoice_JobsView.ItemsSource = ClientJobs;
            }
        }
        

    }

    private async void Admin_Genereate_Invoice_PDF(object sender, EventArgs e)
    {
        var SelectedInvoice = Edit_Invoice_Section.BindingContext as Invoice;

        if (SelectedInvoice != null)
        {
            var response = await client.GetAsync(apiUrl + "Invoices/GeneratePDF/" + SelectedInvoice.ID);

            if (response.IsSuccessStatusCode)
            {
                var pdfbytes = await response.Content.ReadAsByteArrayAsync();

                using var stream = new MemoryStream(pdfbytes);

                var result = await FileSaver.SaveAsync("Invoice_" + SelectedInvoice.ID.ToString() + ".pdf", stream);

                if(result.IsSuccessful)
                {
                    await DisplayAlertAsync("Success", "Invoice Saved.", ":)");
                }
                else
                {
                    await DisplayAlertAsync("Error", "Error while saving Invoice", "):");
                }


            }
        }

    }



}