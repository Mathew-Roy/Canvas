using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(StudentId), "studentId")]
public partial class StudentCoursesView : ContentPage
{
    public string StudentId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
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

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}