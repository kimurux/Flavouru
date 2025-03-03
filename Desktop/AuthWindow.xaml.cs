using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop
{
    public partial class AuthWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly string _credentialsFilePath;
        private readonly byte[] _encryptionKey;

        public AuthWindow()
        {
            InitializeComponent();

            // Initialize HTTP client for API calls
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/api/")
            };

            // Setup encryption key and credentials file path
            _encryptionKey = Encoding.UTF8.GetBytes("FlavoururSecretKey123456789012345");
            _credentialsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Flavouru",
                "credentials.dat");

            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_credentialsFilePath));

            // Load credentials if available
            LoadSavedCredentials();

            // Show the start panel by default if no credentials were loaded
            if (loginPanel.Visibility != Visibility.Visible)
            {
                ShowPanel(startPanel);
            }
        }

        #region Navigation Methods

        private void ShowPanel(UIElement panelToShow)
        {
            // Hide all panels
            startPanel.Visibility = Visibility.Collapsed;
            loginPanel.Visibility = Visibility.Collapsed;
            registerPanel.Visibility = Visibility.Collapsed;

            // Show the requested panel
            panelToShow.Visibility = Visibility.Visible;
        }

        private void btnShowLogin_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel(loginPanel);
        }

        private void btnShowRegister_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel(registerPanel);
        }

        private void btnBackToStart_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel(startPanel);
        }

        private void switchToLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowPanel(loginPanel);
        }

        private void switchToRegister_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowPanel(registerPanel);
        }

        private void forgotPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Implement password recovery functionality
            MessageBox.Show("Функция восстановления пароля будет доступна в будущих обновлениях.",
                           "Восстановление пароля",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information);
        }

        #endregion

        #region Credential Management

        private void LoadSavedCredentials()
        {
            try
            {
                if (File.Exists(_credentialsFilePath))
                {
                    string encryptedData = File.ReadAllText(_credentialsFilePath);
                    string decryptedData = Decrypt(encryptedData);

                    string[] credentials = decryptedData.Split('|');
                    if (credentials.Length == 2)
                    {
                        // Populate the login form
                        txtLoginUsername.Text = credentials[0];
                        txtLoginPassword.Password = credentials[1];
                        chkRememberMe.IsChecked = true;

                        // Show login panel with populated credentials
                        ShowPanel(loginPanel);
                    }
                }
            }
            catch (Exception ex)
            {
                // If there's an error loading credentials, just continue without them
                System.Diagnostics.Debug.WriteLine($"Error loading credentials: {ex.Message}");
            }
        }

        private void SaveCredentials(string username, string password)
        {
            try
            {
                string dataToEncrypt = $"{username}|{password}";
                string encryptedData = Encrypt(dataToEncrypt);
                File.WriteAllText(_credentialsFilePath, encryptedData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving credentials: {ex.Message}");
            }
        }

        private void DeleteSavedCredentials()
        {
            try
            {
                if (File.Exists(_credentialsFilePath))
                {
                    File.Delete(_credentialsFilePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting credentials: {ex.Message}");
            }
        }

        public void ClearCredentials()
        {
            DeleteSavedCredentials();
        }

        private string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.GenerateIV();

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    // Write the IV first
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = _encryptionKey;

                // Get the IV from the cipher text (first 16 bytes)
                byte[] iv = new byte[16];
                Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(cipherBytes, iv.Length, cipherBytes.Length - iv.Length))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        #endregion

        #region Authentication Methods

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginDto = new LoginUserDto
                {
                    Username = txtLoginUsername.Text,
                    Password = txtLoginPassword.Password
                };

                var response = await _httpClient.PostAsJsonAsync("auth/login", loginDto);
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDto>();

                    // Save or delete credentials based on checkbox
                    if (chkRememberMe.IsChecked == true)
                    {
                        SaveCredentials(txtLoginUsername.Text, txtLoginPassword.Password);
                    }
                    else
                    {
                        DeleteSavedCredentials();
                    }

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

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (txtRegisterPassword.Password != txtRegisterConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var registerDto = new RegisterUserDto
                {
                    Username = txtRegisterUsername.Text,
                    Email = txtRegisterEmail.Text,
                    Password = txtRegisterPassword.Password
                };

                var response = await _httpClient.PostAsJsonAsync("auth/register", registerDto);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Регистрация успешна. Теперь вы можете войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    ShowPanel(loginPanel);

                    // Auto-fill the username to improve user experience
                    txtLoginUsername.Text = txtRegisterUsername.Text;
                    txtLoginPassword.Focus();
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

        #endregion
    }
}