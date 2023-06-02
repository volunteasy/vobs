namespace Volunteasy.Core.DTOs;

public record Filter
{
    public (int, int) ReadRange;
    public int Start => ReadRange.Item1;
    public int End => ReadRange.Item2;
    public int Limit => End - Start + 1;
    public int ExceedingLimit => Limit + 1;
    public (int, int) NextPage => (Start + Limit, End + Limit);
};

public record DateRangeFilter(DateTime Start, DateTime End);