namespace MiniZinc.Client;

public readonly struct TimePeriod
{
    public readonly DateTimeOffset Start;
    public readonly DateTimeOffset End;
    public readonly TimeSpan Duration;

    public TimePeriod(DateTimeOffset start)
    {
        Start = start;
        End = start;
        Duration = default;
    }

    public TimePeriod(DateTimeOffset start, DateTimeOffset end)
    {
        if (start > end)
        {
            End = start;
            Start = end;
        }
        else
        {
            Start = start;
            End = end;
        }

        Duration = End - Start;
    }

    public TimePeriod(DateTimeOffset start, TimeSpan duration)
    {
        Start = start;
        End = start + duration;
        Duration = duration;
    }

    public TimePeriod WithStart(DateTimeOffset start) => new(start, End);

    public TimePeriod WithEnd(DateTimeOffset end) => new(Start, end);

    public static TimePeriod Since(DateTimeOffset start) => new(start, DateTimeOffset.Now);
}

public static class TimePeriodExtensions
{
    public static TimePeriod ToPeriod(this DateTimeOffset start, DateTimeOffset end) =>
        new TimePeriod(start, end);

    public static TimePeriod ToPeriod(this DateTimeOffset start, TimeSpan duration) =>
        new TimePeriod(start, duration);
}
