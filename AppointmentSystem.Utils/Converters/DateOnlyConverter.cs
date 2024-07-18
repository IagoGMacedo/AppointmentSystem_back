using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AppointmentSystem.Utils.Converters
{
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() : base(
        dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
        dateTime => DateOnly.FromDateTime(dateTime))
        { }
    }
}
