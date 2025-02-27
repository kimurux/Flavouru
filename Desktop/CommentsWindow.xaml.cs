using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Flavouru.Shared.DTOs;
using System.Threading.Tasks;

namespace Flavouru.Desktop
{
    public partial class CommentsWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly Guid _recipeId;
        private readonly UserDto _currentUser;
        private const string ApiBaseUrl = "http://localhost:5000/api/";
        private const int PageSize = 10;
        private int currentPage = 0;
        private bool isLoading = false;
        private bool hasMoreComments = true;
        private ObservableCollection<CommentDto> Comments { get; set; }

        public CommentsWindow(Guid recipeId, UserDto currentUser)
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };
            _recipeId = recipeId;
            _currentUser = currentUser;
            Comments = new ObservableCollection<CommentDto>();
            commentsListBox.ItemsSource = Comments;

            // Привязываем обработчик прокрутки
            ScrollViewer scrollViewer = GetScrollViewer(commentsListBox);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }

            LoadMoreComments();
        }

        private ScrollViewer GetScrollViewer(DependencyObject element)
        {
            if (element is ScrollViewer)
                return element as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null) return;

            // Проверяем, достигли ли мы конца списка
            if (scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - 50) // 50 пикселей до конца
            {
                if (!isLoading && hasMoreComments)
                {
                    await LoadMoreComments();
                }
            }
        }

        private async Task LoadMoreComments()
        {
            try
            {
                isLoading = true;
                loadingIndicator.Visibility = Visibility.Visible;

                var comments = await _httpClient.GetFromJsonAsync<List<CommentDto>>($"comments/recipe/{_recipeId}?page={currentPage}&pageSize={PageSize}");

                if (comments == null || comments.Count == 0)
                {
                    hasMoreComments = false;
                    return;
                }

                foreach (var comment in comments)
                {
                    Comments.Add(comment);
                }

                currentPage++;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке комментариев: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                isLoading = false;
                loadingIndicator.Visibility = Visibility.Collapsed;
            }
        }

        private async void btnAddComment_Click(object sender, RoutedEventArgs e)
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
                    RecipeId = _recipeId,
                    UserId = _currentUser.Id
                };

                var response = await _httpClient.PostAsJsonAsync("comments", newComment);
                if (response.IsSuccessStatusCode)
                {
                    var createdComment = await response.Content.ReadFromJsonAsync<CommentDto>();
                    Comments.Insert(0, createdComment); 
                    txtNewComment.Clear();
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

