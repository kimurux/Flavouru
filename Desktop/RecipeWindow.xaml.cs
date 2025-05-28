using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop
{
    public partial class RecipeWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly RecipeDto _recipe;
        private readonly bool _isEdit;
        private List<CategoryDto> _allCategories;
        private List<TagDto> _allTags;
        private const string ApiBaseUrl = "http://localhost:5000/api/"; // Добавлен слеш в конце

        public RecipeWindow(RecipeDto recipe)
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };
            _recipe = recipe;
            _isEdit = recipe.Id != Guid.Empty;

            LoadCategoriesAndTags();

            if (_isEdit)
            {
                LoadRecipe();
                Title = "Редактирование рецепта";
            }
            else
            {
                Title = "Новый рецепт";
            }
        }

        private async void LoadCategoriesAndTags()
        {
            try
            {
                _allCategories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("categories");
                _allTags = await _httpClient.GetFromJsonAsync<List<TagDto>>("tags");

                lstCategories.ItemsSource = _allCategories;
                lstCategories.DisplayMemberPath = "Name";

                lstTags.ItemsSource = _allTags;
                lstTags.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке категорий и тегов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecipe()
        {
            txtTitle.Text = _recipe.Title;
            txtDescription.Text = _recipe.Description;
            txtInstructions.Text = _recipe.Instructions;
            txtPrepTime.Text = _recipe.PrepTime.ToString();
            txtCookTime.Text = _recipe.CookTime.ToString();
            txtServings.Text = _recipe.Servings.ToString();

            foreach (var category in _allCategories)
            {
                if (_recipe.Categories.Contains(category.Name))
                {
                    lstCategories.SelectedItems.Add(category);
                }
            }

            foreach (var tag in _allTags)
            {
                if (_recipe.Tags.Contains(tag.Name))
                {
                    lstTags.SelectedItems.Add(tag);
                }
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var recipe = new RecipeDto
                {
                    Id = _isEdit ? _recipe.Id : Guid.NewGuid(),
                    Title = txtTitle.Text,
                    Description = txtDescription.Text,
                    Instructions = txtInstructions.Text,
                    PrepTime = int.Parse(txtPrepTime.Text),
                    CookTime = int.Parse(txtCookTime.Text),
                    Servings = int.Parse(txtServings.Text),
                    UserId = _recipe.UserId,
                    Categories = lstCategories.SelectedItems.Cast<CategoryDto>().Select(c => c.Name).ToList(),
                    Tags = lstTags.SelectedItems.Cast<TagDto>().Select(t => t.Name).ToList()
                };

                HttpResponseMessage response;
                if (_isEdit)
                {
                    response = await _httpClient.PutAsJsonAsync($"/recipes/{recipe.Id}", recipe);
                }
                else
                {
                    response = await _httpClient.PostAsJsonAsync("/recipes", recipe);
                }

                if (response.IsSuccessStatusCode)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении рецепта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

