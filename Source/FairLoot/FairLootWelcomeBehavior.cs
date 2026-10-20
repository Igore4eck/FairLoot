using System.Diagnostics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FairLoot;

/// <summary>
/// One-time overview after character creation. It waits until no other
/// pop-up is open; each companion mod waits a different time (RATO 0.5 s,
/// Imperial Restoration 1.0 s, Fair Loot 1.5 s), so their windows never collide.
/// </summary>
internal sealed class FairLootWelcomeBehavior : CampaignBehaviorBase
{
    private const double QuietSecondsBeforeShowing = 1.5;
    private readonly Stopwatch _quiet = new();
    private bool _pending;

    public override void RegisterEvents()
    {
        CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, () => _pending = FairLootOptions.ShowWelcome);
        CampaignEvents.TickEvent.AddNonSerializedListener(this, _ => TryShow());
    }

    public override void SyncData(IDataStore dataStore)
    {
    }

    private void TryShow()
    {
        if (!_pending)
        {
            return;
        }
        if (InformationManager.IsAnyInquiryActive())
        {
            _quiet.Reset();
            return;
        }
        if (!_quiet.IsRunning)
        {
            _quiet.Start();
        }
        if (_quiet.Elapsed.TotalSeconds < QuietSecondsBeforeShowing)
        {
            return;
        }

        _pending = false;
        TextObject body = new("{=fairloot_welcome_body}Fair Loot: the spoils go to those who earned them, and death to those who asked for it.{newline}{newline}• Ordinary loot after a battle is cut (by default in half): the baggage train is not bottomless, and the soldiers want to eat too.{newline}• Everything worn by enemies you killed yourself is yours: weapons, armor, the horse and its harness. Want plate? Take it off its owner yourself.{newline}• Death depends on the final blow: a weak one more often leaves the victim alive, a strong one kills more often. A blow of {LETHAL} damage or more is fatal for everyone, lords included.{newline}• Knockouts and kills by allies or siege engines do not count toward personal loot: other people's feats are not yours.{newline}{newline}Settings: Mod Options → Fair Loot. There you can also spare your own hero from these rules, if you feel sorry for them.{newline}Details: mod folder Modules/FairLoot, file README.md.{newline}{newline}Loot fairly. Die spectacularly.");
        body.SetTextVariable("LETHAL", FairLootOptions.LethalDamage);
        InformationManager.ShowInquiry(new InquiryData(
            new TextObject("{=fairloot_welcome_title}Fair Loot").ToString(),
            body.ToString(),
            true, false,
            new TextObject("{=fairloot_welcome_ok}Got it").ToString(), string.Empty,
            null, null), true);
    }
}
