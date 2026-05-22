using DuoMusica.ViewModels;

namespace DuoMusica.Views;

public partial class DeathMatchPage : ContentPage
{
    private readonly DeathMatchViewModel _vm;

    public DeathMatchPage(DeathMatchViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }
}
