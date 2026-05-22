using DuoMusica.ViewModels;

namespace DuoMusica.Views;

public partial class SongSearchPage : ContentPage
{
    private readonly SongSearchViewModel _vm;

    public SongSearchPage(SongSearchViewModel vm)
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
