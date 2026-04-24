using ProjectExpenseTrackerHybrid.Models;
using ProjectExpenseTrackerHybrid.Pages;

namespace ProjectExpenseTrackerHybrid.Pages;

public partial class ProjectDetailPage : ContentPage
{
    private readonly ProjectItem _project;

    public ProjectDetailPage(ProjectItem project)
    {
        InitializeComponent();
        _project = project;
        BindingContext = _project;
    }

    private async void ViewExpenses_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ExpenseListPage(_project));
    }
}