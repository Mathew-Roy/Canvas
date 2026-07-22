namespace Maui.Canvas.Views;

public partial class TeacherView : ContentPage
{
    public TeacherView()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}