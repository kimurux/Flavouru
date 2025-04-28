using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly string _credentialsFilePath;
        private readonly byte[] _encryptionKey;

        public LoginWindow()
        {
            InitializeComponent();
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/api/")
            };
            
            _encryptionKey = Encoding.UTF8.GetBytes("FlavoururSecretKey123456789012345");
            _credentialsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Flavouru",
                "credentials.dat");
            
            Directory.CreateDirectory(Path.GetDirectoryName(_credentialsFilePath));
            
            LoadSavedCredentials();
        }

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
                        txtUsername.Text = credentials[0];
                        txtPassword.Password = credentials[1];
                        chkRememberMe.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
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
                    
                    // Save or delete credentials based on checkbox
                    if (chkRememberMe.IsChecked == true)
                    {
                        SaveCredentials(txtUsername.Text, txtPassword.Password);
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

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}