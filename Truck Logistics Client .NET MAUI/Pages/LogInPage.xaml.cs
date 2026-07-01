using System.Diagnostics;
using System.Net.Http.Json;
using TrucksLogisticsClient.Models;
using TrucksLogisticsClient.Pages;

namespace TrucksLogisticsClient
{
    public partial class MainPage : ContentPage
    {

        private string apiUrl;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Clear the token when the page appears
            SecureStorage.Remove("auth_token");

            // set http link for api!!!!!!!!!
            Preferences.Set("api_url", "http://192.168.0.218:5160/api/");
            apiUrl = Preferences.Get("api_url", "127.0.0.1:5160/api/");
        }
        private async void Login_Clicked(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();


                if(string.IsNullOrEmpty(Login_entry.Text) || string.IsNullOrEmpty(Password_entry.Text))
                {
                    LoginResultLabel.Text = "Please enter both username and password.";
                    
                    return;
                }

                LoginResultLabel.Text = "Attempting to log in...";

                var response = await client.PostAsJsonAsync(apiUrl + "Auth/Login", new { Username = Login_entry.Text, Password = Password_entry.Text });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();


                    Users? user = result.User;

                    var token = result.Token;

                    await SecureStorage.SetAsync("auth_token", token);

                    var storedToken = await SecureStorage.GetAsync("auth_token");
                    

                    if (user != null)
                    {

                        LoginResultLabel.Text = string.Empty;

                        
                        if(user.Role == "admin")
                        {
                            await Shell.Current.GoToAsync($"{nameof(MainMenuPage)}?UserID={user.ID}");
                        }
                        else if(user.Role == "user")
                        {
                            await Shell.Current.GoToAsync($"{nameof(UserMainMenuPage)}?UserID={user.ID}");
                        }

                    }
                }
                else
                {
                    LoginResultLabel.Text = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = ex.Message;
                ErrorBorder.IsVisible = true;
                ErrorButton.IsVisible = true;
                ErrorButton.IsEnabled = true;
            }

        }

        private async void Close_Error(object sender, EventArgs e)
        {
            ErrorBorder.IsVisible = false;
            ErrorButton.IsVisible = false;
            ErrorButton.IsEnabled = false;
            ErrorLabel.Text = string.Empty;
        }

        private async void Github_Icon_Clicked(object sender, EventArgs e) 
        {
            await Browser.OpenAsync("https://github.com/NikodemBilinski/Truck-Logistics-Project");
        }

    }
}
