using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FairLoot;

/// <summary>
/// Current settings. They mirror the MCM menu and update as soon as it
/// changes; the defaults below stay in effect if MCM fails to provide them.
/// Hot paths read these cached values, never the MCM instance.
/// </summary>
internal static class FairLootOptions
{
    internal static float OrdinaryLootShare { get; private set; } = 0.5f;
    internal static bool PersonalLoot { get; private set; } = true;
    internal static bool DeathCurve { get; private set; } = true;
    internal static int LethalDamage { get; private set; } = 100;
    internal static bool ApplyToPlayer { get; private set; } = true;
    internal static bool ShowWelcome { get; private set; } = true;
    internal static bool LeaveField { get; private set; } = true;

    private static bool _subscribed;

    internal static void Refresh()
    {
        if (!AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.GetName().Name == "MCMv5"))
        {
            return;
        }
        try
        {
            RefreshFromMcm();
        }
        catch (Exception)
        {
            // A broken or incompatible MCM leaves the defaults in effect.
        }
    }

    // Kept out of line: the JIT must not resolve MCM types unless MCM is loaded.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RefreshFromMcm()
    {
        FairLootSettings? settings = FairLootSettings.Instance;
        if (settings == null)
        {
            return;
        }
        if (!_subscribed)
        {
            settings.PropertyChanged += (_, _) => Refresh();
            _subscribed = true;
        }

        OrdinaryLootShare = Math.Max(0f, Math.Min(1f, settings.OrdinaryLootShare));
        PersonalLoot = settings.PersonalLoot;
        DeathCurve = settings.DeathCurve;
        LethalDamage = Math.Max((int)DeathChanceCurve.TripleDeathDamage + 1, settings.LethalDamage);
        ApplyToPlayer = settings.ApplyToPlayer;
        ShowWelcome = settings.ShowWelcome;
        LeaveField = settings.LeaveField;
    }
}
