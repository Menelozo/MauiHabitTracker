using HabitTracker.Managers;
using HabitTracker.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HabitTracker.Pages
{
    public partial class HomePage : ContentPage, INotifyPropertyChanged
    {
        private readonly DatabaseManager databaseManager;
        private readonly HomePageManager homePageManager;

        public ObservableCollection<HabitItem> Items { get; set; }
        public ObservableCollection<HabitItem> ItemsDone { get; set; }

        public ICommand FrameTappedCommand { get; }

        public HomePage(DatabaseManager _databaseManager)
        {
            InitializeComponent();
            databaseManager = _databaseManager;
            homePageManager = new HomePageManager(databaseManager, this);
            Items = new ObservableCollection<HabitItem>();
            ItemsDone = new ObservableCollection<HabitItem>();
            selectedDateToToday = SelectedDateToToday.Present;
            BindingContext = this;
            PropertyChanged += OnPagePropertyChanged;
            FrameTappedCommand = new Command<HabitItem>(OnFrameTapped);
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            await homePageManager.LoadItemsForSelectedDate(Items, ItemsDone, SelectedDate, selectedDateToToday,
                                                            collectionView1, dayCompleted);
        }

        private async void OnFrameTapped(HabitItem tappedItem)
        {
            if (Items.Contains(tappedItem))
            {
                collectionView1.SelectedItem = tappedItem;
            }
            else if (ItemsDone.Contains(tappedItem))
            {
                collectionView2.SelectedItem = tappedItem;
            }
        }

        private async void CollectionView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is HabitItem item)
            {
                await homePageManager.UpdateHabitDone(item, false, Items, ItemsDone, SelectedDate, selectedDateToToday,
                                                            collectionView1, dayCompleted);
            }
        }

        private async void CollectionView2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is HabitItem item)
            {
                await homePageManager.UpdateHabitDone(item, true, Items, ItemsDone, SelectedDate, selectedDateToToday,
                                                            collectionView1, dayCompleted);
            }
        }

        private DateTime _selectedDate = DateTime.Today;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private void ResetButton_Clicked(object sender, EventArgs e)
        {
            SelectedDate = DateTime.Today;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedDate))
            {
                selectedDateToToday = homePageManager.DefineSelectedDateToToday(SelectedDate, selectedDateToToday);
                homePageManager.EditViewBasedOnSelectedDate(label1, label2, collectionView2, dayCompleted, selectedDateToToday);
                await homePageManager.LoadItemsForSelectedDate(Items, ItemsDone, SelectedDate, selectedDateToToday,
                                                                collectionView1, dayCompleted);
            }
        }

        public enum SelectedDateToToday
        {
            Past,
            Present,
            Future
        }

        SelectedDateToToday selectedDateToToday;
    }
}