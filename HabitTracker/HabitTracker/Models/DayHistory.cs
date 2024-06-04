using Newtonsoft.Json;
using SQLite;
using System.ComponentModel;

namespace HabitTracker.Models
{
    public class DayHistory
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime Day { get; set; }

        private string registeredHabitsSerialized;
        public string RegisteredHabitsSerialized
        {
            get => registeredHabitsSerialized;
            set
            {
                registeredHabitsSerialized = value;
                OnPropertyChanged(nameof(RegisteredHabitsSerialized));

            }
        }

        private Dictionary<int, bool> registeredHabits;
        [Ignore]
        public Dictionary<int, bool> RegisteredHabits
        {
            get
            {
                if (registeredHabits == null)
                {
                    if (string.IsNullOrEmpty(RegisteredHabitsSerialized))
                    {
                        registeredHabits = new Dictionary<int, bool>();
                    }
                    else
                    {
                        registeredHabits = JsonConvert.DeserializeObject<Dictionary<int, bool>>(RegisteredHabitsSerialized);
                    }
                }
                return registeredHabits;
            }
            set
            {
                registeredHabits = value;
                RegisteredHabitsSerialized = JsonConvert.SerializeObject(registeredHabits);
                OnPropertyChanged(nameof(RegisteredHabits));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
