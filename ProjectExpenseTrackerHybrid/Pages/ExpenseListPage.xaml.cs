using ProjectExpenseTrackerHybrid.Models;
using ProjectExpenseTrackerHybrid.Services;
using System.Collections.ObjectModel;

namespace ProjectExpenseTrackerHybrid.Pages;

public partial class ExpenseListPage : ContentPage
{
    private readonly ProjectItem _project;
    private readonly ExpenseService _expenseService = new();
    private readonly ObservableCollection<ExpenseItem> _expenses = new();

    public ExpenseListPage(ProjectItem project)
    {
        InitializeComponent();
        _project = project;
        lblProjectTitle.Text = $"Expenses - {_project.ProjectName}";
        expensesCollectionView.ItemsSource = _expenses;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadExpensesAsync();
    }

    private async Task LoadExpensesAsync()
    {
        try
        {
            ShowLoading(true);

            _expenses.Clear();

            var expenses = await _expenseService.GetExpensesByProjectCodeAsync(_project.ProjectCode);

            foreach (var expense in expenses)
            {
                _expenses.Add(expense);
            }

            UpdateEmptyState();
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

    private async void AddExpense_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddExpensePage(_project));
    }

    private void UpdateEmptyState()
    {
        bool isEmpty = _expenses.Count == 0;
        lblEmptyState.IsVisible = isEmpty;
        expensesCollectionView.IsVisible = !isEmpty;
    }

    private void ShowLoading(bool isLoading)
    {
        loadingIndicator.IsVisible = isLoading;
        loadingIndicator.IsRunning = isLoading;
        expensesCollectionView.IsVisible = !isLoading && _expenses.Count > 0;
        lblEmptyState.IsVisible = !isLoading && _expenses.Count == 0;
    }
}