using System.Globalization;
using System.IO;
using System.Text.Json;

namespace MySchool.Classes
{
    internal class HolidayLogic
    {
        internal enum EventKind
        {
            TermStart,
            TermEnd,
            StaffDevelopment,
            SchoolHoliday
        }

        internal sealed class UpcomingEvent
        {
            public string Title { get; init; } = string.Empty;
            public DateTime Date { get; init; }
            public DateTime? EndDate { get; init; }
            public EventKind Kind { get; init; }

            public override string ToString() => $"{Title} - {Date:d}";
        }

        // Returns the next upcoming events (term starts/ends and staff development days) based on the QLD holidays JSON file.
        internal static IReadOnlyList<UpcomingEvent> GetUpcomingEvents(int take = 3, DateTime? fromDate = null)
        {
            try
            {
                var path = GetHolidaysJsonPath();
                if (!File.Exists(path))
                {
                    return Array.Empty<UpcomingEvent>();
                }

                using var doc = JsonDocument.Parse(File.ReadAllText(path));
                var root = doc.RootElement;

                var results = new List<UpcomingEvent>(16);
                var pivot = (fromDate ?? DateTime.Today).Date;

                foreach (var yearProp in root.EnumerateObject())
                {
                    if (!int.TryParse(yearProp.Name, out var year)) continue;
                    var yearObj = yearProp.Value;

                    // term_dates
                    if (yearObj.TryGetProperty("term_dates", out var termDates))
                    {
                        foreach (var termProp in termDates.EnumerateObject())
                        {
                            var termName = NormalizeTermName(termProp.Name);
                            var term = termProp.Value;
                            if (term.TryGetProperty("start", out var startEl))
                            {
                                if (TryParseIsoDate(startEl.GetString(), out var start))
                                {
                                    results.Add(new UpcomingEvent
                                    {
                                        Title = $"{termName} Starts",
                                        Date = start,
                                        Kind = EventKind.TermStart
                                    });
                                }
                            }
                            if (term.TryGetProperty("end", out var endEl))
                            {
                                if (TryParseIsoDate(endEl.GetString(), out var end))
                                {
                                    results.Add(new UpcomingEvent
                                    {
                                        Title = $"{termName} Ends",
                                        Date = end,
                                        Kind = EventKind.TermEnd
                                    });
                                }
                            }
                        }

                        // school_holidays: add a single event per term with start-end range
                        if (yearObj.TryGetProperty("school_holidays", out var holidays))
                        {
                            foreach (var termProp in holidays.EnumerateObject())
                            {
                                var termName = NormalizeTermName(termProp.Name);
                                var termNode = termProp.Value;
                                if (termNode.TryGetProperty("start", out var sEl) && termNode.TryGetProperty("end", out var eEl))
                                {
                                    if (TryParseIsoDate(sEl.GetString(), out var s) && TryParseIsoDate(eEl.GetString(), out var e))
                                    {
                                        results.Add(new UpcomingEvent
                                        {
                                            Title = $"{termName} Holidays",
                                            Date = s,
                                            EndDate = e,
                                            Kind = EventKind.SchoolHoliday
                                        });
                                    }
                                }
                            }
                        }
                    }

                    // staff_development_days (structure may vary per year/term)
                    if (yearObj.TryGetProperty("staff_development_days", out var sdd))
                    {
                        foreach (var termProp in sdd.EnumerateObject())
                        {
                            var termName = NormalizeTermName(termProp.Name);
                            var termNode = termProp.Value;

                            // range: start + end
                            if (termNode.TryGetProperty("start", out var sEl) && termNode.TryGetProperty("end", out var eEl))
                            {
                                if (TryParseIsoDate(sEl.GetString(), out var s) && TryParseIsoDate(eEl.GetString(), out var e))
                                {
                                    // Add a single aggregated event covering the range
                                    results.Add(new UpcomingEvent
                                    {
                                        Title = $"{termName} Staff Development Day",
                                        Date = s,
                                        EndDate = e,
                                        Kind = EventKind.StaffDevelopment
                                    });
                                }
                            }

                            // single date
                            if (termNode.TryGetProperty("date", out var dateEl))
                            {
                                if (TryParseIsoDate(dateEl.GetString(), out var d))
                                {
                                    results.Add(new UpcomingEvent
                                    {
                                        Title = $"{termName} Staff Development Day",
                                        Date = d,
                                        Kind = EventKind.StaffDevelopment
                                    });
                                }
                            }

                            // multiple dates
                            if (termNode.TryGetProperty("dates", out var datesEl) && datesEl.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var item in datesEl.EnumerateArray())
                                {
                                    if (item.ValueKind == JsonValueKind.String && TryParseIsoDate(item.GetString(), out var d))
                                    {
                                        results.Add(new UpcomingEvent
                                        {
                                            Title = $"{termName} Staff Development Day",
                                            Date = d,
                                            Kind = EventKind.StaffDevelopment
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                var upcoming = results
                    .Where(e => e.Date.Date >= pivot)
                    .OrderBy(e => e.Date)
                    .ThenBy(e => e.Kind)
                    .Take(Math.Max(0, take))
                    .ToList();

                return upcoming;
            }
            catch
            {
                // Fail silent and return empty to avoid breaking UI
                return Array.Empty<UpcomingEvent>();
            }
        }

        internal static string FormatDate(DateTime date, DateTime? endDate = null)
        {
            var culture = new CultureInfo("en-AU");
            if (endDate == null || endDate.Value.Date == date.Date)
            {
                return date.Year == DateTime.Today.Year
                    ? date.ToString("d MMMM", culture)
                    : date.ToString("d MMMM yyyy", culture);
            }

            var end = endDate.Value.Date;
            // Same month and year: 26-27 January 2026 (or without year if current year)
            if (date.Year == end.Year && date.Month == end.Month)
            {
                var left = date.ToString("d", culture);
                var right = end.ToString(date.Year == DateTime.Today.Year ? "d MMMM" : "d MMMM yyyy", culture);
                return $"{left}-{right}";
            }

            // Same year but different months: 30 Sep - 02 Oct 2026 (or without year if current year)
            if (date.Year == end.Year)
            {
                var left = date.ToString("d MMM", culture);
                var right = end.ToString(date.Year == DateTime.Today.Year ? "d MMM" : "d MMM yyyy", culture);
                return $"{left} - {right}";
            }

            // Different years: 13 Dec 2025 - 26 Jan 2026
            return $"{date:d MMM yyyy} - {end:d MMM yyyy}";
        }

        private static string GetHolidaysJsonPath()
        {
            // Prefer file copied to output directory
            var fromOutput = Path.Combine(AppContext.BaseDirectory, "resources", "data", "holidays", "QLD.json");
            if (File.Exists(fromOutput)) return fromOutput;

            // Fallback to project relative path (design-time)
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var fallback = Path.Combine(baseDir, "resources", "data", "holidays", "QLD.json");
            if (File.Exists(fallback)) return fallback;

            // Last resort: check environment variable for custom path
            var envPath = Environment.GetEnvironmentVariable("QLD_HOLIDAYS_JSON_PATH");
            if (!string.IsNullOrWhiteSpace(envPath) && File.Exists(envPath)) return envPath;
            throw new FileNotFoundException("Could not find QLD.json in any known location. Set the QLD_HOLIDAYS_JSON_PATH environment variable to specify a custom path.");
        }

        private static string NormalizeTermName(string termKey)
        {
            // Convert keys like "term_1" to "Term 1"
            if (string.IsNullOrWhiteSpace(termKey)) return "Term";
            var parts = termKey.Split('_');
            if (parts.Length == 2 && int.TryParse(parts[1], out var n))
            {
                return $"Term {n}";
            }
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(termKey.Replace('_', ' '));
        }

        private static bool TryParseIsoDate(string? s, out DateTime date)
        {
            if (!string.IsNullOrWhiteSpace(s) && DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var d))
            {
                date = d.Date;
                return true;
            }
            date = default;
            return false;
        }

        // unused helper to enumerate each day in a date range
        private static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
    }
}
