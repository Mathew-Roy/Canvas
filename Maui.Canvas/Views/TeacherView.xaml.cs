using System.Linq;
using Library.Canvas.Models;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

public partial class TeacherView : ContentPage
{
    public TeacherView()
    {
        InitializeComponent();
        BindingContext = new TeacherCoursesViewModel();
    }

    private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Course course)
        {
            ((CollectionView)sender).SelectedItem = null;
            await Shell.Current.GoToAsync($"teachercourse?courseId={course.Id}");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}