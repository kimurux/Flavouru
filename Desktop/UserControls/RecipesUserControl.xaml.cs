using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop.UserControls
{
    public partial class RecipesUserControl : UserControl
    {
        private readonly HttpClient _httpClient;
        private UserDto _currentUser;

        public RecipesUserControl()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/api/")
            };
        }

        public void Initialize(UserDto currentUser)
        {
            _currentUser = currentUser;
        }

        public async void LoadRecipes()
        {
            try
            {
                var recipes = await _httpClient.GetFromJsonAsync<List<RecipeDto>>("recipes");
                recipesListView.ItemsSource = recipes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке рецептов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void recipesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (recipesListView.SelectedItem is RecipeDto selectedRecipe)
            {
                var recipeDetailWindow = new RecipeDetailWindow(selectedRecipe, _currentUser);
                recipeDetailWindow.ShowDialog();
            }
        }
    }
}

