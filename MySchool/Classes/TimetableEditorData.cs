using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MySchool.Classes
{
    /// <summary>
    /// View model for editing a single period in the timetable editor
    /// </summary>
    public class EditablePeriod : INotifyPropertyChanged
    {
    private int _periodNumber;
        private string _subject = string.Empty;
        private string _teacher = string.Empty;
        private string _room = string.Empty;
        private string _startTime = string.Empty;
      private string _endTime = string.Empty;
        private bool _isBreak;

 public int PeriodNumber
        {
  get => _periodNumber;
     set { _periodNumber = value; OnPropertyChanged(); }
        }

        public string Subject
    {
            get => _subject;
          set { _subject = value; OnPropertyChanged(); }
        }

        public string Teacher
        {
            get => _teacher;
  set { _teacher = value; OnPropertyChanged(); }
     }

        public string Room
        {
      get => _room;
       set { _room = value; OnPropertyChanged(); }
        }

        public string StartTime
        {
            get => _startTime;
         set { _startTime = value; OnPropertyChanged(); }
      }

        public string EndTime
  {
            get => _endTime;
       set { _endTime = value; OnPropertyChanged(); }
        }

        public bool IsBreak
        {
 get => _isBreak;
   set { _isBreak = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
     }

        /// <summary>
        /// Convert to Period model for saving
   /// </summary>
        public Period ToPeriod()
      {
  return new Period
  {
                PeriodNumber = PeriodNumber,
          Subject = Subject,
    Teacher = Teacher,
       Room = Room,
                StartTime = StartTime,
 EndTime = EndTime,
    IsBreak = IsBreak
            };
        }

        /// <summary>
   /// Create from Period model
        /// </summary>
      public static EditablePeriod FromPeriod(Period period)
        {
            return new EditablePeriod
 {
              PeriodNumber = period.PeriodNumber,
 Subject = period.Subject,
                Teacher = period.Teacher,
      Room = period.Room,
       StartTime = period.StartTime,
  EndTime = period.EndTime,
  IsBreak = period.IsBreak
            };
        }
    }

    /// <summary>
    /// View model for editing a day's schedule in the timetable editor
    /// </summary>
    public class EditableDaySchedule : INotifyPropertyChanged
    {
        private string _day = string.Empty;
private ObservableCollection<EditablePeriod> _periods = new();

        public string Day
   {
      get => _day;
            set { _day = value; OnPropertyChanged(); }
        }

  public ObservableCollection<EditablePeriod> Periods
        {
     get => _periods;
            set { _periods = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

   protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Convert to DaySchedule model for saving
        /// </summary>
        public DaySchedule ToDaySchedule()
        {
         return new DaySchedule
     {
         Day = Day,
      Periods = Periods.Select(p => p.ToPeriod()).ToList()
};
        }

        /// <summary>
        /// Create from DaySchedule model
        /// </summary>
    public static EditableDaySchedule FromDaySchedule(DaySchedule daySchedule)
{
         return new EditableDaySchedule
   {
                Day = daySchedule.Day,
   Periods = new ObservableCollection<EditablePeriod>(
                daySchedule.Periods.Select(EditablePeriod.FromPeriod))
         };
 }
    }
}
