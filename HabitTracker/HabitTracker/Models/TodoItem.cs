using Newtonsoft.Json;
using SQLite;
using System.ComponentModel;

namespace HabitTracker.Models
{
    public class TodoItem : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }

        private string daysOfWeekSerialized;
        public string DaysOfWeekSerialized
        {
            get => daysOfWeekSerialized;
            set
            {
                daysOfWeekSerialized = value;
                OnPropertyChanged(nameof(DaysOfWeek));
            }
        }

        private Dictionary<string, bool> daysOfWeek;
        [Ignore]
        public Dictionary<string, bool> DaysOfWeek
        {
            get
            {
                if (daysOfWeek == null)
                {
                    if (string.IsNullOrEmpty(DaysOfWeekSerialized))
                    {
                        daysOfWeek = new Dictionary<string, bool>
                        {
                            { "Monday", false },
                            { "Tuesday", false },
                            { "Wednesday", false },
                            { "Thursday", false },
                            { "Friday", false },
                            { "Saturday", false },
                            { "Sunday", false }
                        };
                    }
                    else
                    {
                        daysOfWeek = JsonConvert.DeserializeObject<Dictionary<string, bool>>(DaysOfWeekSerialized);
                    }
                }
                return daysOfWeek;
            }
            set
            {
                daysOfWeek = value;
                DaysOfWeekSerialized = JsonConvert.SerializeObject(daysOfWeek);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
