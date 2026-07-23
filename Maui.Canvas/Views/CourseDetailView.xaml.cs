using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(StudentId), "studentId")]
public partial class CourseDetailView : ContentPage
{
    private int _courseId;
    private int _studentId;

    public string CourseId
    {
        set { int.TryParse(value, out _courseId); TryLoad(); }
    }

    public string StudentId
    {
        set { int.TryParse(value, out _studentId); TryLoad(); }
    }

    public CourseDetailView()
    {
        InitializeComponent();
    }

    private void TryLoad()
    {
        if (_courseId > 0 && _studentId > 0)
        {
            var vm = new CourseDetailViewModel();
            vm.Load(_courseId, _studentId);
            BindingContext = vm;
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}