using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using Flavouru.Shared.DTOs;

namespace Flavouru.Desktop.UserControls
{
    public partial class TagsUserControl : UserControl
    {
        private readonly HttpClient _httpClient;

        public TagsUserControl()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/api/")
            };
        }

        public async void LoadTags()
        {
            try
            {
                var tags = await _httpClient.GetFromJsonAsync<List<TagDto>>("tags");
                tagsListBox.ItemsSource = tags;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке тегов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnAddTag_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewTag.Text))
            {
                MessageBox.Show("Пожалуйста, введите название тега", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newTag = new CreateTagDto
                {
                    Name = txtNewTag.Text
                };

                var response = await _httpClient.PostAsJsonAsync("tags", newTag);
                if (response.IsSuccessStatusCode)
                {
                    txtNewTag.Clear();
                    LoadTags();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении тега", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeleteTag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TagDto tag)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить тег '{tag.Name}'?", "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"tags/{tag.Id}");
                        if (response.IsSuccessStatusCode)
                        {
                            LoadTags();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при удалении тега", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

