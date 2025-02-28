using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop
{
    public partial class RecipeDetailWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly RecipeDto _recipe;
        private readonly UserDto _currentUser;
        private const string ApiBaseUrl = "http://localhost:5000/api/";

        public RecipeDetailWindow(RecipeDto recipe, UserDto currentUser)
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };
            _recipe = recipe;
            _currentUser = currentUser;
            DataContext = _recipe;
        }

        private void btnViewComments_Click(object sender, RoutedEventArgs e)
        {
            var commentsWindow = new CommentsWindow(_recipe.Id, _currentUser);
            commentsWindow.Show();
        }

        private async void btnSendComment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var comment = new CreateCommentDto
                {
                    Content = txtComment.Text,
                    RecipeId = _recipe.Id,
                    UserId = _currentUser.Id
                };

                var response = await _httpClient.PostAsJsonAsync("comments", comment);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Комментарий успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtComment.Clear();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении комментария", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

