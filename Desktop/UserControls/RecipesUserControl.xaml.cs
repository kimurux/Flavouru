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
    public partial class RecipesUserControl : UserControl
    {
        private readonly HttpClient _httpClient;
        private UserDto _currentUser;
        private List<RecipeDto> _allRecipes;

        public RecipesUserControl()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/api/")
            };

            // Set up event handlers for search and filter
            txtSearch.TextChanged += OnSearchFilterChanged;
            cmbFilter.SelectionChanged += OnSearchFilterChanged;
        }

        public void Initialize(UserDto currentUser)
        {
            _currentUser = currentUser;
        }

        public async void LoadRecipes()
        {
            try
            {
                // Show loading indicator
                loadingIndicator.Visibility = Visibility.Visible;
                recipesItemsControl.Visibility = Visibility.Collapsed;
                txtEmptyState.Visibility = Visibility.Collapsed;

                // Load recipes from API
                _allRecipes = await _httpClient.GetFromJsonAsync<List<RecipeDto>>("recipes");

                // Apply search/filter and update UI
                UpdateRecipesList();

                // Hide loading indicator
                loadingIndicator.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                loadingIndicator.Visibility = Visibility.Collapsed;
                MessageBox.Show($"Ошибка при загрузке рецептов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                // For testing when API is unavailable
                _allRecipes = new List<RecipeDto>
                {
                    new RecipeDto {
                        Id = Guid.NewGuid(),
                        Title = "Delicious Pasta",
                        Description = "A simple and tasty pasta dish",
                        PrepTime = 10,
                        CookTime = 20,
                        Servings = 4
                    },
                    new RecipeDto {
                        Id = Guid.NewGuid(),
                        Title = "test",
                        Description = "test2",
                        PrepTime = 5,
                        CookTime = 5,
                        Servings = 3
                    },
                    new RecipeDto {
                        Id = Guid.NewGuid(),
                        Title = "testuser123",
                        Description = "testuser123",
                        PrepTime = 2,
                        CookTime = 2,
                        Servings = 2
                    }
                };

                UpdateRecipesList();
            }
        }

        private void UpdateRecipesList()
        {
            if (_allRecipes == null)
                return;

            // Apply search filter
            var searchText = txtSearch.Text?.ToLower() ?? string.Empty;
            var filteredRecipes = _allRecipes
                .Where(r => string.IsNullOrEmpty(searchText) ||
                            r.Title.ToLower().Contains(searchText) ||
                            r.Description.ToLower().Contains(searchText))
                .ToList();

            // Apply sorting based on selected filter
            switch (cmbFilter.SelectedIndex)
            {
                case 1: // By date
                    filteredRecipes = filteredRecipes.OrderByDescending(r => r.CreatedAt).ToList();
                    break;
                case 2: // Alphabetically
                    filteredRecipes = filteredRecipes.OrderBy(r => r.Title).ToList();
                    break;
                default:
                    // Default ordering, could be by ID or another property
                    break;
            }

            // Update UI
            recipesItemsControl.ItemsSource = filteredRecipes;

            // Show or hide empty state message
            if (filteredRecipes.Count == 0)
            {
                recipesItemsControl.Visibility = Visibility.Collapsed;
                txtEmptyState.Visibility = Visibility.Visible;
            }
            else
            {
                recipesItemsControl.Visibility = Visibility.Visible;
                txtEmptyState.Visibility = Visibility.Collapsed;
            }
        }

        private void OnSearchFilterChanged(object sender, EventArgs e)
        {
            UpdateRecipesList();
        }

        private void btnViewRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RecipeDto selectedRecipe)
            {
                var recipeDetailWindow = new RecipeDetailWindow(selectedRecipe, _currentUser);
                recipeDetailWindow.ShowDialog();

                // Reload recipes after dialog closes in case of updates
                LoadRecipes();
            }
        }
    }
}