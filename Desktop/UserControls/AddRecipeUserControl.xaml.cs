using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop.UserControls
{
    public partial class AddRecipeUserControl : UserControl
    {
        private readonly HttpClient _httpClient;
        private UserDto _currentUser;

        public AddRecipeUserControl()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/api/")
            };
        }

        public void Initialize(UserDto user)
        {
            _currentUser = user;
            LoadCategoriesAndTags();
        }

        private async void LoadCategoriesAndTags()
        {
            try
            {
                var categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("categories");
                var tags = await _httpClient.GetFromJsonAsync<List<TagDto>>("tags");

                lstCategories.ItemsSource = categories;
                lstCategories.DisplayMemberPath = "Name";

                lstTags.ItemsSource = tags;
                lstTags.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке категорий и тегов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnSaveRecipe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newRecipe = new CreateRecipeDto
                {
                    Title = txtTitle.Text,
                    Description = txtDescription.Text,
                    Instructions = txtInstructions.Text,
                    PrepTime = int.Parse(txtPrepTime.Text),
                    CookTime = int.Parse(txtCookTime.Text),
                    Servings = int.Parse(txtServings.Text),
                    UserId = _currentUser.Id,
                    CategoryIds = lstCategories.SelectedItems.Cast<CategoryDto>().Select(c => c.Id).ToList(),
                    TagIds = lstTags.SelectedItems.Cast<TagDto>().Select(t => t.Id).ToList()
                };

                var response = await _httpClient.PostAsJsonAsync("recipes", newRecipe);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Рецепт успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении рецепта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            txtTitle.Clear();
            txtDescription.Clear();
            txtInstructions.Clear();
            txtPrepTime.Clear();
            txtCookTime.Clear();
            txtServings.Clear();
            lstCategories.SelectedItems.Clear();
            lstTags.SelectedItems.Clear();
        }
    }
}

