using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

public partial class SemesterManagementView : ContentPage
{
    public SemesterManagementView()
    {
        InitializeComponent();
        Reload();
    }

    private void Reload()
    {
        var vm = new SemesterManagementViewModel();
        vm.Load();
        BindingContext = vm;
    }

    private void OnAddSemester(object sender, EventArgs e)
    {
        string name = NewSemesterName.Text;
        if (string.IsNullOrWhiteSpace(name)) return;

        var list = SemesterServiceProxy.Current.Semesters;
        int newId = list.Any() ? list.Max(s => s.Id) + 1 : 1;
        list.Add(new Semester { Id = newId, Name = name });

        NewSemesterName.Text = string.Empty;
        Reload();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}