using System.Globalization;

namespace HabitTracker.Converters
{
    public class TrueRegisteredHabitsCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Dictionary<int, bool> registeredHabits)
            {
                int trueCount = 0;
                foreach (var habit in registeredHabits)
                {
                    if (habit.Value)
                        trueCount++;
                }
                return trueCount;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
