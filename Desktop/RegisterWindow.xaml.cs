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
    public partial class RegisterWindow : Window
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "http://localhost:5000/api/";

        public RegisterWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };
        }

        private async Task<bool> CheckServerConnection()
        {
            try
            {
                Debug.WriteLine("Проверка подключения к серверу...");
                // Используем конкретный endpoint вместо пустого URL
                var response = await _httpClient.GetAsync("/auth");
                Debug.WriteLine($"Ответ сервера: {response.StatusCode}");

                // Даже если endpoint не найден (404), сервер все равно работает
                return response.StatusCode != System.Net.HttpStatusCode.ServiceUnavailable;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Ошибка при проверке подключения: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Неожиданная ошибка при проверке подключения: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                if (txtPassword.Password != txtConfirmPassword.Password)
                {
                    MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var registerDto = new RegisterUserDto
                {
                    Username = txtUsername.Text,
                    Email = txtEmail.Text,
                    Password = txtPassword.Password
                };

                string requestContent = JsonSerializer.Serialize(registerDto);
                Debug.WriteLine($"Отправка запроса на регистрацию: {requestContent}");
                Debug.WriteLine($"URL запроса: {_httpClient.BaseAddress}auth/register");

                var response = await _httpClient.PostAsJsonAsync("auth/register", registerDto);

                Debug.WriteLine($"Получен ответ от сервера. Статус код: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDto>();
                    Debug.WriteLine($"Успешная регистрация пользователя: {user.Username}");
                    MessageBox.Show("Регистрация успешна! Теперь вы можете войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Ошибка регистрации. Содержание ответа: {errorContent}");
                    MessageBox.Show(
                        $"Ошибка регистрации:\n\n" +
                        $"Статус: {response.StatusCode}\n\n" +
                        $"URL: {_httpClient.BaseAddress}auth/register\n\n" +
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
                Debug.WriteLine($"Исключение при регистрации: {ex}");
                MessageBox.Show(
                    $"Произошла ошибка при попытке регистрации:\n\n" +
                    $"Тип ошибки: {ex.GetType().Name}\n\n" +
                    $"Сообщение: {ex.Message}\n\n" +
                    $"Stack Trace:\n{ex.StackTrace}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void btnBackToLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}

