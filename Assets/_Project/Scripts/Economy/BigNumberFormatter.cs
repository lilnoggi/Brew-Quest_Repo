public static class BigNumberFormatter
{
    // The array of suffixes
    private static readonly string[] _suffixes = { "", "K", "M", "B", "T", "Qa", "Qi" };

    public static string FormatValue(double value)
    {
        // If value is less than 1000, just show the normal number with zero decimal places
        if (value < 1000)
        {
            return value.ToString("F0");
        }

        int i = 0;

        // Divide by 1000 until the number is small enough to read, and track how many times we divided to determine the suffix
        while (value >= 1000 && i < _suffixes.Length - 1)
        {
            i++;
            value /= 1000;
        }

        // Return the shrunken number with 2 decimal places (F2) and the correct letter suffix
        return value.ToString("F2") + _suffixes[i];
    }
}
