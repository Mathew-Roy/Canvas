using System.Linq;
using Library.Canvas.Models;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(StudentId), "studentId")]
public partial class StudentCoursesView : ContentPage
{
    private int _studentId;

    public string StudentId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                _studentId = id;
                var vm = new StudentCoursesViewModel();
                vm.Load(id);
                BindingContext = vm;
            }
        }
    }

    public StudentCoursesView()
    {
        InitializeComponent();
    }

    private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Course course)
        {
            ((CollectionView)sender).SelectedItem = null;
            await Shell.Current.GoToAsync($"coursedetail?courseId={course.Id}&studentId={_studentId}");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
    private async void OnMainMenuClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}