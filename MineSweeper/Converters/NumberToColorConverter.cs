using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MineSweeper.Converters
{
    public class NumberToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int number)
            {
                switch (number)
                {
                    case 1: return new SolidColorBrush(Colors.Blue);
                    case 2: return new SolidColorBrush(Colors.Green);
                    case 3: return new SolidColorBrush(Colors.Red);
                    case 4: return new SolidColorBrush(Colors.Purple);
                    case 5: return new SolidColorBrush(Colors.Maroon);
                    case 6: return new SolidColorBrush(Colors.Turquoise);
                    case 7: return new SolidColorBrush(Colors.Black);
                    case 8: return new SolidColorBrush(Colors.Gray);
                    default: return new SolidColorBrush(Colors.Black);
                }
            }

            throw new ArgumentException("Expected value to be of type int");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("NumberToColorConverter can only be used for one way conversion.");
        }
    }
}
