using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient;

        public LoginWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/api/")
            };
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginDto = new LoginUserDto
                {
                    Username = txtUsername.Text,
                    Password = txtPassword.Password
                };

                var response = await _httpClient.PostAsJsonAsync("auth/login", loginDto);
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDto>();
                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

