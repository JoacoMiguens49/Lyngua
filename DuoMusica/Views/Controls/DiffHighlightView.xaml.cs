using Lyngua.Models;
using System.Collections.ObjectModel;

namespace Lyngua.Views.Controls;

public partial class DiffHighlightView : ContentView
{
    public static readonly BindableProperty DiffTokensProperty =
        BindableProperty.Create(
            nameof(DiffTokens),
            typeof(IEnumerable<DiffToken>),
            typeof(DiffHighlightView),
            null,
            propertyChanged: OnTokensChanged);

    public IEnumerable<DiffToken> DiffTokens
    {
        get => (IEnumerable<DiffToken>)GetValue(DiffTokensProperty);
        set => SetValue(DiffTokensProperty, value);
    }

    public DiffHighlightView()
    {
        InitializeComponent();
    }

    private static void OnTokensChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DiffHighlightView view && newValue is IEnumerable<DiffToken> tokens)
            view.Render(tokens);
    }

    private void Render(IEnumerable<DiffToken> tokens)
    {
        var formatted = new FormattedString();

        foreach (var token in tokens)
        {
            var color = token.Status switch
            {
                DiffStatus.Correct => Color.FromArgb("#00B894"),
                DiffStatus.Close => Color.FromArgb("#FDCB6E"),
                DiffStatus.Wrong => Color.FromArgb("#D63031"),
                DiffStatus.Missing => Color.FromArgb("#636E72"),
                _ => Colors.White
            };

            formatted.Spans.Add(new Span
            {
                Text = token.Text + " ",
                TextColor = color,
                FontSize = 16,
                TextDecorations = token.Status == DiffStatus.Missing
                    ? TextDecorations.Strikethrough
                    : TextDecorations.None
            });
        }

        DiffLabel.FormattedText = formatted;
    }
}
