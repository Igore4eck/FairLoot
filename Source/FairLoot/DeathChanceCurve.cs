using System;

namespace FairLoot;

internal static class DeathChanceCurve
{
    // Final-blow damage anchors. At or below WeakHit the BASE survival chance
    // is multiplied (x5 soldiers, x3 heroes); Double/Triple multiply the BASE
    // death chance (soldiers x2/x3, heroes x1.5/x2); Lethal and above always kills.
    internal const float WeakHitDamage = 8f;
    internal const float DoubleDeathDamage = 40f;
    internal const float TripleDeathDamage = 65f;
    // Default certain-death damage; the active value comes from FairLootOptions.
    internal const float LethalDamage = 100f;

    // Soldiers: a final blow of exactly this much damage keeps the BASE
    // (medicine/perk) death chance; weaker blows spare, stronger ones kill more.
    internal const int SoldierBalanceDamage = 25;

    internal static float Adjust(float baseDeathChance, int finalBlowDamage, bool isHero = false, float lethalDamage = LethalDamage)
    {
        lethalDamage = Math.Max(TripleDeathDamage + 1f, lethalDamage);
        float baseline = Clamp01(baseDeathChance);
        if (finalBlowDamage <= 0)
        {
            return baseline;
        }

        float weakHit = Clamp01(1f - (isHero ? 3f : 5f) * (1f - baseline));
        float atDouble = Clamp01((isHero ? 1.5f : 2f) * baseline);
        float atTriple = Clamp01((isHero ? 2f : 3f) * baseline);

        if (finalBlowDamage <= WeakHitDamage)
        {
            return weakHit;
        }
        if (!isHero && finalBlowDamage < SoldierBalanceDamage)
        {
            return Lerp(weakHit, baseline, (finalBlowDamage - WeakHitDamage) / (SoldierBalanceDamage - WeakHitDamage));
        }
        if (!isHero && finalBlowDamage < DoubleDeathDamage)
        {
            return Lerp(baseline, atDouble, (finalBlowDamage - SoldierBalanceDamage) / (DoubleDeathDamage - SoldierBalanceDamage));
        }
        if (finalBlowDamage < DoubleDeathDamage)
        {
            return Lerp(weakHit, atDouble, (finalBlowDamage - WeakHitDamage) / (DoubleDeathDamage - WeakHitDamage));
        }
        if (finalBlowDamage < TripleDeathDamage)
        {
            return Lerp(atDouble, atTriple, (finalBlowDamage - DoubleDeathDamage) / (TripleDeathDamage - DoubleDeathDamage));
        }
        if (finalBlowDamage < lethalDamage)
        {
            return Lerp(atTriple, 1f, (finalBlowDamage - TripleDeathDamage) / (lethalDamage - TripleDeathDamage));
        }
        return 1f;
    }

    // Damage where the interpolated death chance equals the unmodified
    // medicine/perk chance. Fixed for soldiers; varies with the base for heroes.
    internal static float? BalanceDamage(float baseDeathChance, bool isHero = false)
    {
        float baseline = Clamp01(baseDeathChance);
        if (baseline <= 0f || baseline >= 1f)
        {
            return null;
        }
        if (!isHero)
        {
            return SoldierBalanceDamage;
        }

        float weakHit = Clamp01(1f - 3f * (1f - baseline));
        float atDouble = Clamp01(1.5f * baseline);
        if (Math.Abs(atDouble - weakHit) < 0.000001f)
        {
            return null;
        }
        return WeakHitDamage + (DoubleDeathDamage - WeakHitDamage) * (baseline - weakHit) / (atDouble - weakHit);
    }

    private static float Clamp01(float value) => Math.Max(0f, Math.Min(1f, value));
    private static float Lerp(float from, float to, float t) => from + (to - from) * t;
}
