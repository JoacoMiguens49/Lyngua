namespace Lyngua.Helpers;

public static class XpCalculator
{
    private static readonly int[] LevelThresholds =
        { 0, 200, 500, 1000, 2000, 3500, 5500, 8000, 11000, 15000, int.MaxValue };

    public static int CalculateSegmentXp(double score, bool isFirstAttempt, bool isPerfect)
    {
        var base_ = (int)(100 * Math.Pow(score / 100.0, 2));

        if (isPerfect) base_ = (int)(base_ * 2.0);
        else if (isFirstAttempt) base_ = (int)(base_ * 1.5);

        return base_;
    }

    public static int GetLevel(int totalXp)
    {
        for (int i = LevelThresholds.Length - 2; i >= 0; i--)
        {
            if (totalXp >= LevelThresholds[i])
                return i + 1;
        }
        return 1;
    }

    public static (int currentXp, int requiredXp) GetLevelProgress(int totalXp)
    {
        var level = GetLevel(totalXp);
        var idx = level - 1;
        var start = idx < LevelThresholds.Length ? LevelThresholds[idx] : 0;
        var end = idx + 1 < LevelThresholds.Length ? LevelThresholds[idx + 1] : int.MaxValue;

        return (totalXp - start, end - start);
    }
}
