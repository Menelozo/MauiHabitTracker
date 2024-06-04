using Newtonsoft.Json;
using SQLite;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace HabitTracker.Models
{
    public class HabitItem : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }


        [ForeignKey("TodoItem")]
        public int TodoItemId { get; set; }

        private TodoItem todoItem;
        [Ignore]
        public TodoItem TodoItem
        {
            get => todoItem;
            set
            {
                todoItem = value;
                TodoItemId = value?.ID ?? 0;
                OnPropertyChanged(nameof(TodoItem));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string doneOnSerialized;
        public string DoneOnSerialized
        {
            get => doneOnSerialized;
            set
            {
                if (doneOnSerialized != value)
                {
                    doneOnSerialized = value;
                    OnPropertyChanged(nameof(DoneOnSerialized));
                }
            }
        }
        private List<DateOnly> doneOn;
        [Ignore]
        public List<DateOnly> DoneOn
        {
            get
            {
                if (doneOn == null)
                {
                    doneOn = string.IsNullOrEmpty(DoneOnSerialized)
                             ? new List<DateOnly>()
                             : JsonConvert.DeserializeObject<List<DateOnly>>(DoneOnSerialized);
                }
                return doneOn;
            }
            set
            {
                if (doneOn != value)
                {
                    doneOn = value;
                    DoneOnSerialized = JsonConvert.SerializeObject(doneOn);
                    OnPropertyChanged(nameof(DoneOn));
                }
            }
        }

        private string alarmsSerialized;
        public string AlarmsSerialized
        {
            get => alarmsSerialized;
            set
            {
                alarmsSerialized = value;
            }
        }

        private Dictionary<string, List<DateTime>> alarms;
        [Ignore]
        public Dictionary<string, List<DateTime>> Alarms
        {
            get
            {
                if (alarms == null)
                {
                    if(string.IsNullOrEmpty(AlarmsSerialized))
                    {
                        alarms = new Dictionary<string, List<DateTime>>
                        {
                            { "Monday", new List<DateTime>() },
                            { "Tuesday", new List<DateTime>() },
                            { "Wednesday", new List<DateTime>() },
                            { "Thursday", new List<DateTime>() },
                            { "Friday", new List<DateTime>() },
                            { "Saturday", new List<DateTime>() },
                            { "Sunday", new List<DateTime>() }
                        };
                    }
                    else
                    {
                        alarms = JsonConvert.DeserializeObject<Dictionary<string, List<DateTime>>>(AlarmsSerialized);
                    }
                }
                return alarms;
            }
            set
            {
                alarms = value;
                AlarmsSerialized = JsonConvert.SerializeObject(alarms);
            }
        }
    }
}
