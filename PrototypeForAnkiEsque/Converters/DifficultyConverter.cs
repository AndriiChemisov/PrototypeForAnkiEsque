using System.Globalization;
using System.Windows.Data;

namespace PrototypeForAnkiEsque.Converters
{
    public class DifficultyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int easeRating)
            {
                // The percentage calculation based on the ease rating (0 is easy, 2 is hard)
                int totalCards = 100; // Assuming 100% represents all easy cards
                int difficulty = (2 - easeRating) * 50; // Convert rating to a scale of 0 to 100
                return $"{100 - difficulty}%"; // Show percentage
            }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; // We don't need to convert back for this scenario
        }
    }
}
