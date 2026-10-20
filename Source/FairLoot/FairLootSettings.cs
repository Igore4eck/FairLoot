using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace FairLoot;

// MCM page. MCM is a required dependency: this type derives from an MCM
// class, and the game reads every type of the DLL when it loads the module.
// Only FairLootOptions touches it.
public sealed class FairLootSettings : AttributeGlobalSettings<FairLootSettings>
{
    public override string Id => "FairLoot_v1";
    // MCM localizes item names, hints and groups itself, but shows the page name as given.
    public override string DisplayName => new TextObject("{=fairloot_mcm_title}Fair Loot").ToString();
    public override string FolderName => "FairLoot";
    public override string FormatType => "json2";

    [SettingPropertyFloatingInteger("{=fairloot_mcm_loot_share}Ordinary loot kept", 0f, 1f, "#0%", Order = 0, RequireRestart = false,
        HintText = "{=fairloot_mcm_loot_share_hint}Share of the ordinary post-battle loot that stays. 50% means half of the items are removed. Your personal loot is added on top.")]
    [SettingPropertyGroup("{=fairloot_mcm_group_loot}Loot", GroupOrder = 0)]
    public float OrdinaryLootShare { get; set; } = 0.5f;

    [SettingPropertyBool("{=fairloot_mcm_personal}Personal loot", Order = 1, RequireRestart = false,
        HintText = "{=fairloot_mcm_personal_hint}After a victory you get the full equipment of every enemy you killed yourself, including horse and harness. Knockouts, allies and siege engines do not count.")]
    [SettingPropertyGroup("{=fairloot_mcm_group_loot}Loot", GroupOrder = 0)]
    public bool PersonalLoot { get; set; } = true;

    [SettingPropertyBool("{=fairloot_mcm_death}Final blow decides death", Order = 0, RequireRestart = false,
        HintText = "{=fairloot_mcm_death_hint}The chance to die depends on the damage of the last hit: weak blows mostly knock out, strong ones kill more often. Medicine and perks still set the base chance.")]
    [SettingPropertyGroup("{=fairloot_mcm_group_death}Death in battle", GroupOrder = 1)]
    public bool DeathCurve { get; set; } = true;

    [SettingPropertyInteger("{=fairloot_mcm_lethal}Certain-death damage", 70, 300, "0", Order = 1, RequireRestart = false,
        HintText = "{=fairloot_mcm_lethal_hint}A final blow of at least this much damage always kills, even lords who normally cannot die.")]
    [SettingPropertyGroup("{=fairloot_mcm_group_death}Death in battle", GroupOrder = 1)]
    public int LethalDamage { get; set; } = 100;

    [SettingPropertyBool("{=fairloot_mcm_player}Apply to your hero", Order = 2, RequireRestart = false,
        HintText = "{=fairloot_mcm_player_hint}When off, your own hero keeps the game's standard death rules and is not affected by Fair Loot.")]
    [SettingPropertyGroup("{=fairloot_mcm_group_death}Death in battle", GroupOrder = 1)]
    public bool ApplyToPlayer { get; set; } = true;

    [SettingPropertyBool("{=fairloot_mcm_leave}Leave the field and watch", Order = 1, RequireRestart = false,
        HintText = "{=fairloot_mcm_leave_hint}Past the red line your hero leaves the field alive; the battle goes on under AI, you watch.")]
    [SettingPropertyGroup("{=fairloot_mcm_group_misc}Misc", GroupOrder = 2)]
    public bool LeaveField { get; set; } = true;

    [SettingPropertyBool("{=fairloot_mcm_welcome}Welcome message", Order = 0, RequireRestart = false,
        HintText = "{=fairloot_mcm_welcome_hint}Show a short overview of the mod when a new campaign starts.")]
    [SettingPropertyGroup("{=fairloot_mcm_group_misc}Misc", GroupOrder = 2)]
    public bool ShowWelcome { get; set; } = true;
}
