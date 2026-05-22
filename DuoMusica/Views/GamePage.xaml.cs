using DuoMusica.ViewModels;

namespace DuoMusica.Views;

public partial class GamePage : ContentPage
{
    private readonly GameViewModel _vm;

    public GamePage(GameViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _vm.OnReturningFromResults();
        if (_vm.Song is not null && _vm.CurrentSegment is null)
            await _vm.InitializeAsync();
    }
}
