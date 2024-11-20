using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GLFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string sessionToken = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(username.Text);
            Debug.WriteLine(passwordbox.Password);
            sessionToken = await LoginUser(username.Text, passwordbox.Password);

            if (!string.IsNullOrEmpty(sessionToken))
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Login failed. Please check your credentials.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<string> LoginUser(string username, string password)
        {
            // Initialize the HTTP client & set the required headers.
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Parse-Application-Id", "");
            client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", "");

            // Create the request message content.
            var content = new StringContent($"{{\"username\":\"{username}\",\"password\":\"{password}\"}}", System.Text.Encoding.UTF8, "application/json");

            // Send POST request to back4app's login endpoint.
            var response = await client.PostAsync("https://parseapi.back4app.com/login", content);

            if (response.IsSuccessStatusCode)
            {
                // Get response.
                string responseBody = await response.Content.ReadAsStringAsync();

                // Create JSON.
                var json = JsonObject.Parse(responseBody);

                // Return the `sessionToken` (get it from the JSON).
                return json["sessionToken"].ToString();
            }
            else
            {
                Debug.WriteLine($"Login failed : {response.StatusCode}, {response.ReasonPhrase}");
                return null;
            }
        }

        private void username_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            // Check if the session token is valid
            if (!string.IsNullOrEmpty(sessionToken))
            {
                string gameExecutablePath = "D:\\Faculta\\Master\\GH\\Games-PAJV\\Lab3\\Build\\GameLauncher.exe"; // Replace with your game's path

                var startInfo = new ProcessStartInfo
                {
                    FileName = gameExecutablePath,
                    Arguments = $"--sessionToken={sessionToken}",
                    UseShellExecute = false
                };

                try
                {
                    Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to start the game: {ex.Message}");
                    MessageBox.Show($"Failed to start the game. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please log in before starting the game.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}