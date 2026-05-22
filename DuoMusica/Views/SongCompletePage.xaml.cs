using DuoMusica.ViewModels;

namespace DuoMusica.Views;

public partial class SongCompletePage : ContentPage
{
    private readonly SongCompleteViewModel _vm;

    public SongCompletePage(SongCompleteViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.OnAppearingAsync();
    }
}
