using System;
using FairLoot;

static void Check(float actual, float expected, string label)
{
    if (Math.Abs(actual - expected) > 0.00001f)
    {
        throw new Exception($"{label}: expected {expected}, got {actual}");
    }
}

const float baseDeathChance = 0.25f;
Check(DeathChanceCurve.Adjust(baseDeathChance, 7), 0f, "weak hit");
Check(DeathChanceCurve.Adjust(baseDeathChance, 8), 0f, "8 damage anchor");
Check(DeathChanceCurve.Adjust(baseDeathChance, 25), baseDeathChance, "balance point");
Check(DeathChanceCurve.Adjust(0.6f, 25), 0.6f, "balance point with medicine");
Check(DeathChanceCurve.Adjust(baseDeathChance, 40), 0.5f, "40 damage anchor");
Check(DeathChanceCurve.Adjust(baseDeathChance, 65), 0.75f, "65 damage anchor");
Check(DeathChanceCurve.Adjust(baseDeathChance, 100), 1f, "100 damage anchor");
Check(DeathChanceCurve.Adjust(0f, 101), 1f, "100+ overrides regular troop survival");
Check(DeathChanceCurve.BalanceDamage(baseDeathChance)!.Value, 25f, "balance damage");
Check(DeathChanceCurve.Adjust(baseDeathChance, 7, isHero: true), 0f, "hero weak hit");
Check(DeathChanceCurve.Adjust(baseDeathChance, 40, isHero: true), 0.375f, "hero 40 damage anchor");
Check(DeathChanceCurve.Adjust(baseDeathChance, 65, isHero: true), 0.5f, "hero 65 damage anchor");
Check(DeathChanceCurve.Adjust(0f, 101, isHero: true), 1f, "immune hero 100+ hit");
Check(DeathChanceCurve.BalanceDamage(baseDeathChance, isHero: true)!.Value, 29.333334f, "hero balance damage");

foreach (float baseline in new[] { 0.1f, 0.25f, 0.5f, 0.6f, 0.8f, 0.9f })
{
    float previous = -1f;
    for (int damage = 1; damage <= 110; damage++)
    {
        float current = DeathChanceCurve.Adjust(baseline, damage);
        if (current < previous - 0.00001f || current < 0f || current > 1f)
        {
            throw new Exception($"Curve out of range or decreasing: base={baseline}, hit={damage}");
        }
        previous = current;
    }
    Console.WriteLine($"Base death {baseline:P0}: neutral final blow at {DeathChanceCurve.BalanceDamage(baseline):F1} HP");
    float previousHero = -1f;
    for (int damage = 1; damage <= 110; damage++)
    {
        float current = DeathChanceCurve.Adjust(baseline, damage, isHero: true);
        if (current < previousHero - 0.00001f || current < 0f || current > 1f)
        {
            throw new Exception($"Hero curve out of range or decreasing: base={baseline}, hit={damage}");
        }
        previousHero = current;
    }
    Console.WriteLine($"Hero base death {baseline:P0}: neutral final blow at {DeathChanceCurve.BalanceDamage(baseline, isHero: true):F1} HP");
}

Console.WriteLine("Death chance curve checks passed.");
