using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop
{
    public partial class CategoriesWindow : Window
    {
        private readonly HttpClient _httpClient;

        public CategoriesWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/api")
            };
            LoadCategories();
        }

        private async void LoadCategories()
        {
            try
            {
                var categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("/categories");
                categoriesListBox.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewCategory.Text))
            {
                MessageBox.Show("Пожалуйста, введите название категории", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newCategory = new CreateCategoryDto
                {
                    Name = txtNewCategory.Text
                };

                var response = await _httpClient.PostAsJsonAsync("/categories", newCategory);
                if (response.IsSuccessStatusCode)
                {
                    txtNewCategory.Clear();
                    LoadCategories();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении категории", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is CategoryDto category)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить категорию '{category.Name}'?", "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"/categories/{category.Id}");
                        if (response.IsSuccessStatusCode)
                        {
                            LoadCategories();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при удалении категории", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}

