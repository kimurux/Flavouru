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
            LoadRecipeDetails();
            LoadComments();
        }

        private void LoadRecipeDetails()
        {
            txtTitle.Text = _recipe.Title;
            txtDescription.Text = _recipe.Description;
            txtPrepTime.Text = _recipe.PrepTime.ToString();
            txtCookTime.Text = _recipe.CookTime.ToString();
            txtServings.Text = _recipe.Servings.ToString();
            txtInstructions.Text = _recipe.Instructions;
            txtCategories.Text = string.Join(", ", _recipe.Categories);
            txtTags.Text = string.Join(", ", _recipe.Tags);
        }

        private async void LoadComments()
        {
            try
            {
                var comments = await _httpClient.GetFromJsonAsync<List<CommentDto>>($"comments/recipe/{_recipe.Id}");
                commentsListBox.ItemsSource = comments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке комментариев: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnSendComment_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewComment.Text))
            {
                MessageBox.Show("Пожалуйста, введите комментарий", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newComment = new CreateCommentDto
                {
                    Content = txtNewComment.Text,
                    RecipeId = _recipe.Id,
                    UserId = _currentUser.Id
                };

                var response = await _httpClient.PostAsJsonAsync("/comments", newComment);
                if (response.IsSuccessStatusCode)
                {
                    txtNewComment.Clear();
                    LoadComments();
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

