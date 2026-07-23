using System.Linq;
using Library.Canvas.Models;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

public partial class StudentView : ContentPage
{
    public StudentView()
    {
        InitializeComponent();
        BindingContext = new StudentsViewModel();
    }

    private async void OnStudentSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Student student)
        {
            ((CollectionView)sender).SelectedItem = null; // clear so re-selecting works later
            await Shell.Current.GoToAsync($"studentcourses?studentId={student.Id}");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}