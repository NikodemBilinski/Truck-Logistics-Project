using System.Diagnostics;
using System.Net.Http.Json;
using TrucksLogisticsClient.Models;
using TrucksLogisticsClient.Pages;

namespace TrucksLogisticsClient
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }
        private async void Login_Clicked(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();

                var url = "http://192.168.0.218:5160/api/Auth/Login";

                if(string.IsNullOrEmpty(Login_entry.Text) || string.IsNullOrEmpty(Password_entry.Text))
                {
                    LoginResultLabel.Text = "Please enter both username and password.";
                    count++;
                    if(count >= 10)
                    {
                        LoginResultLabel.Text = "stop spamming it you dumbass";
                        count = 0;
                    }
                        return;
                }

                var response = await client.PostAsJsonAsync(url, new { Username = Login_entry.Text.ToString(), Password = Password_entry.Text.ToString() });

                if (response.IsSuccessStatusCode)
                {
                    Users? user = await response.Content.ReadFromJsonAsync<Users>();
                    if (user != null)
                    {
                        LoginResultLabel.Text = "Successfully logged in!";

                        
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
