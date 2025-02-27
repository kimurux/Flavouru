using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Flavouru.Shared.DTOs;
using System.Text.Json;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Flavouru.Desktop
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "http://localhost:5000/api/";

        public LoginWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };
            Debug.WriteLine($"Инициализирован BaseAddress: {ApiBaseUrl}");
        }

        private async Task<bool> CheckServerConnection()
        {
            try
            {
                Debug.WriteLine("Проверка подключения к серверу...");
                var response = await _httpClient.GetAsync("auth/login");
                Debug.WriteLine($"Ответ сервера при проверке подключения: {response.StatusCode}");

                return response.StatusCode != System.Net.HttpStatusCode.ServiceUnavailable;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Ошибка при проверке подключения: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Неожиданная ошибка при проверке подключения: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    MessageBox.Show("Пожалуйста, введите имя пользователя и пароль", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!await CheckServerConnection())
                {
                    MessageBox.Show(
                        "Не удалось подключиться к серверу. Пожалуйста, проверьте:\n\n" +
                        "1. Запущен ли сервер API\n" +
                        "2. Доступен ли адрес http://localhost:5000\n" +
                        "3. Нет ли проблем с сетевым подключением",
                        "Ошибка подключения",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return;
                }

                var loginDto = new LoginUserDto
                {
                    Username = txtUsername.Text,
                    Password = txtPassword.Password
                };

                string requestContent = JsonSerializer.Serialize(loginDto);
                Debug.WriteLine($"Отправка запроса на вход: {requestContent}");
                Debug.WriteLine($"URL запроса: {_httpClient.BaseAddress}auth/login");

                var response = await _httpClient.PostAsJsonAsync("auth/login", loginDto);

                Debug.WriteLine($"Получен ответ от сервера. Статус код: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDto>();
                    Debug.WriteLine($"Успешный вход пользователя: {user.Username}");
                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Ошибка входа. Содержание ответа: {errorContent}");
                    MessageBox.Show(
                        $"Ошибка входа:\n\n" +
                        $"Статус: {response.StatusCode}\n\n" +
                        $"URL: {_httpClient.BaseAddress}auth/login\n\n" +
                        $"Отправленные данные:\n{requestContent}\n\n" +
                        $"Ответ сервера:\n{errorContent}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Исключение при входе: {ex}");
                MessageBox.Show(
                    $"Произошла ошибка при попытке входа:\n\n" +
                    $"Тип ошибки: {ex.GetType().Name}\n\n" +
                    $"Сообщение: {ex.Message}\n\n" +
                    $"Stack Trace:\n{ex.StackTrace}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}

