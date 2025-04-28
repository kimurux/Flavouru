using System.Windows;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly UserDto _currentUser;

        public MainWindow(UserDto user)
        {
            InitializeComponent();
            _currentUser = user;
            txtWelcome.Text = $"Добро пожаловать, {_currentUser.Username}!";
            recipesControl.Initialize(_currentUser);
            ShowRecipes();
        }

        private void btnRecipes_Click(object sender, RoutedEventArgs e)
        {
            ShowRecipes();
        }

        private void btnAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            HideAllControls();
            addRecipeControl.Visibility = Visibility.Visible;
            addRecipeControl.Initialize(_currentUser);
        }

        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
            HideAllControls();
            categoriesControl.Visibility = Visibility.Visible;
            categoriesControl.LoadCategories();
        }

        private void btnTags_Click(object sender, RoutedEventArgs e)
        {
            HideAllControls();
            tagsControl.Visibility = Visibility.Visible;
            tagsControl.LoadTags();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var startWindow = new AuthWindow();
            startWindow.Show();
            this.Close();
        }

        private void ShowRecipes()
        {
            HideAllControls();
            recipesControl.Visibility = Visibility.Visible;
            recipesControl.LoadRecipes();
        }

        private void HideAllControls()
        {
            recipesControl.Visibility = Visibility.Collapsed;
            addRecipeControl.Visibility = Visibility.Collapsed;
            categoriesControl.Visibility = Visibility.Collapsed;
            tagsControl.Visibility = Visibility.Collapsed;
        }
    }
}

