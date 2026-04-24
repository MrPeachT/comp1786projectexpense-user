using ProjectExpenseTrackerHybrid.Models;
using ProjectExpenseTrackerHybrid.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProjectExpenseTrackerHybrid;

public partial class MainPage : ContentPage
{
    private readonly ObservableCollection<ProjectItem> _allProjects = new();
    private readonly ObservableCollection<ProjectItem> _filteredProjects = new();
    private readonly ProjectService _projectService = new();

    public MainPage()
    {
        InitializeComponent();
        projectsCollectionView.ItemsSource = _filteredProjects;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_allProjects.Count == 0)
        {
            await LoadProjectsFromFirestoreAsync();
        }
    }

    private async Task LoadProjectsFromFirestoreAsync()
    {
        try
        {
            ShowLoading(true);

            _allProjects.Clear();
            _filteredProjects.Clear();

            var projects = await _projectService.GetProjectsAsync();

            foreach (var project in projects)
            {
                project.IsFavourite = Preferences.Get($"fav_{project.ProjectCode}", false);
                _allProjects.Add(project);
            }

            ApplySearch(searchBarProjects.Text ?? string.Empty);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
            UpdateEmptyState();
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private void FavouriteButton_Clicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not ProjectItem project)
        {
            return;
        }

        project.IsFavourite = !project.IsFavourite;
        Preferences.Set($"fav_{project.ProjectCode}", project.IsFavourite);
    }

    private void SearchBarProjects_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplySearch(e.NewTextValue ?? string.Empty);
    }

    private void ApplySearch(string keyword)
    {
        _filteredProjects.Clear();

        string trimmedKeyword = keyword.Trim().ToLower();

        var matchingProjects = string.IsNullOrWhiteSpace(trimmedKeyword)
            ? _allProjects
            : new ObservableCollection<ProjectItem>(
                _allProjects.Where(p =>
                    (p.ProjectName ?? "").ToLower().Contains(trimmedKeyword) ||
                    (p.ProjectCode ?? "").ToLower().Contains(trimmedKeyword) ||
                    (p.StartDate ?? "").ToLower().Contains(trimmedKeyword))
        );

        foreach (var project in matchingProjects)
        {
            _filteredProjects.Add(project);
        }

        UpdateEmptyState();
    }


    private async void ViewDetail_Clicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not ProjectItem project)
        {
            return;
        }

        await Navigation.PushAsync(new ProjectExpenseTrackerHybrid.Pages.ProjectDetailPage(project));
    }

    private async void RefreshProjects_Clicked(object sender, EventArgs e)
    {
        await LoadProjectsFromFirestoreAsync();
    }

    private void UpdateEmptyState()
    {
        bool isEmpty = _filteredProjects.Count == 0;
        lblEmptyState.IsVisible = isEmpty;
        projectsCollectionView.IsVisible = !isEmpty;
    }

    private void ShowLoading(bool isLoading)
    {
        loadingIndicator.IsVisible = isLoading;
        loadingIndicator.IsRunning = isLoading;
        projectsCollectionView.IsVisible = !isLoading && _filteredProjects.Count > 0;
        lblEmptyState.IsVisible = !isLoading && _filteredProjects.Count == 0;
    }
}