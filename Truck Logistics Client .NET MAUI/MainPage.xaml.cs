using System.Net.Http.Json;

namespace TrucksLogisticsClient
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }



        private async void Get_Trucks(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();

            var url = "http://192.168.0.218:5160/api/Values/Get_Trucks";

            var response = await client.GetFromJsonAsync<List<Truck>>(url);

            if(response != null)
            {
                TrucksLabel.Text = string.Empty;
                foreach(var truck in response)
                {
                    TrucksLabel.Text += truck.Id + " " + truck.Owner + " " + truck.Capacity + " " + truck.IsBusy + "\n";
                }
            }


        }
    }
}
