using Maui.Canvas.ViewModels;

namespace Maui.Canvas;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }

    private async void OnTeacherClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("teacher");
    }

    private async void OnStudentClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("student");
    }
}