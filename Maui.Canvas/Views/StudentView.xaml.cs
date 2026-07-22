namespace Maui.Canvas.Views;

public partial class StudentView : ContentPage
{
    public StudentView()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}