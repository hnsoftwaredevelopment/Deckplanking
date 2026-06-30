namespace DeckPlanking.Core.Measurement;

public sealed record ScaleSettings(ScaleMode Mode, decimal DecimalRatio, decimal InchesPerFoot)
{
    public static ScaleSettings DecimalScale(decimal ratio)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ratio);
        return new ScaleSettings(ScaleMode.Decimal, ratio, 0m);
    }

    public static ScaleSettings ImperialInchesPerFoot(decimal inchesPerFoot)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(inchesPerFoot);
        return new ScaleSettings(ScaleMode.ImperialInchesPerFoot, 0m, inchesPerFoot);
    }
}
