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
        private bool _isDragging;
        private Action? _onTimeChanged;

        public int PeriodNumber
        {
            get => _periodNumber;
            internal set { _periodNumber = value; OnPropertyChanged(); }
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
            set
            {
                if (_startTime != value)
                {
                    _startTime = value; OnPropertyChanged();
                    _onTimeChanged?.Invoke();
                }
            }
        }

        public string EndTime
        {
            get => _endTime;
            set
            {
                if (_endTime != value)
                {
                    _endTime = value; OnPropertyChanged();
                    _onTimeChanged?.Invoke();
                }
            }
        }

        public bool IsBreak
        {
            get => _isBreak;
            set { _isBreak = value; OnPropertyChanged(); }
        }

        public bool IsDragging
        {
            get => _isDragging;
            set { _isDragging = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Get the start time as TimeSpan for comparison
        /// </summary>
        public TimeSpan GetStartTimeSpan()
        {
            return TimeSpan.TryParse(StartTime, out var time) ? time : TimeSpan.Zero;
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

        /// <summary>
        /// Set the callback to be invoked when start or end time changes
        /// </summary>
        public void SetTimeChangedCallback(Action callback)
        {
            _onTimeChanged = callback;
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

        /// <summary>
        /// Automatically reorder periods based on start time and renumber them sequentially
        /// </summary>
        public void AutoReorderByTime()
        {
            if (Periods.Count == 0) return;

            // Sort periods by start time
            var sortedPeriods = Periods.OrderBy(p => p.GetStartTimeSpan()).ToList();

            // Clear and re-add in sorted order
            Periods.Clear();
            for (int i = 0; i < sortedPeriods.Count; i++)
            {
                sortedPeriods[i].PeriodNumber = i + 1;
                Periods.Add(sortedPeriods[i]);
            }
        }

        /// <summary>
        /// Get the expected period number for a period based on its start time
        /// </summary>
        public int GetExpectedPeriodNumber(EditablePeriod period)
        {
            var sortedPeriods = Periods.OrderBy(p => p.GetStartTimeSpan()).ToList();
            return sortedPeriods.IndexOf(period) + 1;
        }
    }
}
