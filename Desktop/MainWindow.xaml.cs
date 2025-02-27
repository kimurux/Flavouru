using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "http://localhost:5000/api/";
        private UserDto _currentUser;

        public MainWindow(UserDto user)
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };
            _currentUser = user;
            txtWelcome.Text = $"Добро пожаловать, {_currentUser.Username}!";
            LoadRecipes();
        }

        private async void LoadRecipes()
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

        private void btnAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            var newRecipe = new RecipeDto { UserId = _currentUser.Id };
            var recipeWindow = new RecipeWindow(newRecipe);
            if (recipeWindow.ShowDialog() == true)
            {
                LoadRecipes();
            }
        }

        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
            var categoriesWindow = new CategoriesWindow();
            categoriesWindow.ShowDialog();
        }

        private void btnTags_Click(object sender, RoutedEventArgs e)
        {
            var tagsWindow = new TagsWindow();
            tagsWindow.ShowDialog();
        }

        private async void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/logout", _currentUser.Id);
                if (response.IsSuccessStatusCode)
                {
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при выходе из системы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void recipesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (recipesListView.SelectedItem is RecipeDto selectedRecipe)
            {
                var recipeDetailWindow = new RecipeDetailWindow(selectedRecipe, _currentUser);
                recipeDetailWindow.Show();
            }
        }
    }
}

