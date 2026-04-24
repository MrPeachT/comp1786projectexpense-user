using ProjectExpenseTrackerHybrid.Models;
using ProjectExpenseTrackerHybrid.Services;
using System.Globalization;

namespace ProjectExpenseTrackerHybrid.Pages;

public partial class AddExpensePage : ContentPage
{
    private readonly ProjectItem _project;
    private readonly ExpenseService _expenseService = new();

    public AddExpensePage(ProjectItem project)
    {
        InitializeComponent();
        _project = project;
    }

    private async void SaveExpense_Clicked(object sender, EventArgs e)
    {
        try
        {
            ShowLoading(true);

            string expenseId = entryExpenseId.Text?.Trim() ?? string.Empty;
            string dateOfExpense = (datePickerExpense.Date ?? DateTime.Today).ToString("yyyy-MM-dd");
            string amountText = entryAmount.Text?.Trim() ?? string.Empty;
            string currency = entryCurrency.Text?.Trim() ?? string.Empty;
            string expenseType = pickerExpenseType.SelectedItem?.ToString() ?? string.Empty;
            string paymentMethod = pickerPaymentMethod.SelectedItem?.ToString() ?? string.Empty;
            string claimant = entryClaimant.Text?.Trim() ?? string.Empty;
            string paymentStatus = pickerPaymentStatus.SelectedItem?.ToString() ?? string.Empty;
            string description = editorDescription.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(expenseId))
            {
                await DisplayAlert("Validation Error", "Expense ID is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(amountText))
            {
                await DisplayAlert("Validation Error", "Amount is required.", "OK");
                return;
            }

            if (!double.TryParse(amountText, out double amount) || amount <= 0)
            {
                await DisplayAlert("Validation Error", "Amount must be a valid number greater than 0.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(currency))
            {
                await DisplayAlert("Validation Error", "Currency is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(expenseType))
            {
                await DisplayAlert("Validation Error", "Expense Type is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                await DisplayAlert("Validation Error", "Payment Method is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(claimant))
            {
                await DisplayAlert("Validation Error", "Claimant is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(paymentStatus))
            {
                await DisplayAlert("Validation Error", "Payment Status is required.", "OK");
                return;
            }

            var expense = new ExpenseItem
            {
                ExpenseId = expenseId,
                ProjectCode = _project.ProjectCode,
                DateOfExpense = dateOfExpense,
                Amount = amount,
                Currency = currency,
                ExpenseType = expenseType,
                PaymentMethod = paymentMethod,
                Claimant = claimant,
                PaymentStatus = paymentStatus,
                Description = description,
                ImagePath = string.Empty,
                LastModified = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Synced = 1
            };

            await _expenseService.AddExpenseAsync(_project.ProjectCode, expense);

            await DisplayAlert("Success", "Expense added successfully.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private void ShowLoading(bool isLoading)
    {
        loadingIndicator.IsVisible = isLoading;
        loadingIndicator.IsRunning = isLoading;
    }
}