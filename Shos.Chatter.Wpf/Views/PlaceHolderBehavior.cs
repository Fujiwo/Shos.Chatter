using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Shos.Chatter.Wpf.Views
{
    public static class PlaceHolderBehavior
    {
        public static readonly DependencyProperty PlaceHolderTextProperty = DependencyProperty.RegisterAttached(
            "PlaceHolderText",
            typeof(string),
            typeof(PlaceHolderBehavior),
            new PropertyMetadata(null, OnPlaceHolderChanged));

        static void OnPlaceHolderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox is null)
                return;

            var placeHolder = e.NewValue as string;
            var handler     = CreateEventHandler(placeHolder);
            if (string.IsNullOrEmpty(placeHolder)) {
                textBox.TextChanged -= handler;
            } else {
                textBox.TextChanged += handler;
                if (string.IsNullOrEmpty(textBox.Text))
                    textBox.Background = CreateVisualBrush(placeHolder);
            }
        }

        static TextChangedEventHandler CreateEventHandler(string placeHolder)
            => (sender, e) => ((TextBox)sender).Background = string.IsNullOrEmpty(((TextBox)sender).Text) ? (Brush)CreateVisualBrush(placeHolder)
                                                                                                          : new SolidColorBrush(Colors.Transparent);

        static VisualBrush CreateVisualBrush(string placeHolder)
            => new VisualBrush(
                new Label() {
                    Content             = placeHolder,
                    Padding             = new Thickness(5, 1, 1, 1),
                    Foreground          = new SolidColorBrush(Colors.LightGray),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment   = VerticalAlignment.Center,
                }) {
                    Stretch    = Stretch.None,
                    TileMode   = TileMode.None,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Center,
                };

        public static void SetPlaceHolderText(TextBox textBox, string placeHolder)
            => textBox.SetValue(PlaceHolderTextProperty, placeHolder);

        public static string? GetPlaceHolderText(TextBox textBox)
            => textBox.GetValue(PlaceHolderTextProperty) as string;
    }
}
