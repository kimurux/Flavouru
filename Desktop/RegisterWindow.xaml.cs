using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop
{
    public partial class RegisterWindow : Window
    {
        private readonly HttpClient _httpClient;

        public RegisterWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/api/")
            };
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var registerDto = new RegisterUserDto
                {
                    Username = txtUsername.Text,
                    Email = txtEmail.Text,
                    Password = txtPassword.Password
                };

                var response = await _httpClient.PostAsJsonAsync("auth/register", registerDto);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Регистрация успешна. Теперь вы можете войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка регистрации: {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

