using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MySchool.Classes
{
    public class TimetableData
    {
        [JsonPropertyName("timetable")]
        public List<DaySchedule> Timetable { get; set; } = new();

        [JsonPropertyName("metadata")]
        public TimetableMetadata Metadata { get; set; } = new();
    }

    public class DaySchedule
    {
        [JsonPropertyName("day")]
        public string Day { get; set; } = string.Empty;

        [JsonPropertyName("periods")]
        public List<Period> Periods { get; set; } = new();
    }

    public class Period
    {
        [JsonPropertyName("periodNumber")]
        public int PeriodNumber { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; } = string.Empty;

        [JsonPropertyName("teacher")]
        public string Teacher { get; set; } = string.Empty;

        [JsonPropertyName("room")]
        public string Room { get; set; } = string.Empty;

        [JsonPropertyName("startTime")]
        public string StartTime { get; set; } = string.Empty;

        [JsonPropertyName("endTime")]
        public string EndTime { get; set; } = string.Empty;

        [JsonPropertyName("isBreak")]
        public bool IsBreak { get; set; } = false;
    }

    public class TimetableMetadata
    {
        [JsonPropertyName("schoolName")]
        public string SchoolName { get; set; } = string.Empty;

        [JsonPropertyName("term")]
        public string Term { get; set; } = string.Empty;

        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("validFrom")]
        public string ValidFrom { get; set; } = string.Empty;

        [JsonPropertyName("validTo")]
        public string ValidTo { get; set; } = string.Empty;
    }
}
