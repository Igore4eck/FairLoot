using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace FairLoot;

internal sealed class FinalBlowDeathModel : AgentDecideKilledOrUnconsciousModel
{
    private readonly AgentDecideKilledOrUnconsciousModel _previous;
    private readonly PersonalLootSession _session;

    internal FinalBlowDeathModel(AgentDecideKilledOrUnconsciousModel previous,
        PersonalLootSession session)
    {
        _previous = previous;
        _session = session;
    }

    public override float GetAgentStateProbability(Agent affectorAgent, Agent effectedAgent,
        DamageTypes damageType, WeaponFlags weaponFlags, out float useSurgeryProbability)
    {
        float baseline = _previous.GetAgentStateProbability(affectorAgent, effectedAgent,
            damageType, weaponFlags, out useSurgeryProbability);

        if (!FairLootOptions.DeathCurve || effectedAgent == null || !effectedAgent.IsHuman ||
            !_session.TryGetLastHitDamage(effectedAgent, out int finalBlowDamage))
        {
            return baseline;
        }

        if (!FairLootOptions.ApplyToPlayer && effectedAgent.Character == Hero.MainHero?.CharacterObject)
        {
            return baseline;
        }

        int lethalDamage = FairLootOptions.LethalDamage;
        if (effectedAgent.IsHero && finalBlowDamage < lethalDamage &&
            effectedAgent.Character is CharacterObject character &&
            !character.HeroObject.CanDie(KillCharacterAction.KillCharacterActionDetail.DiedInBattle))
        {
            return baseline;
        }

        return DeathChanceCurve.Adjust(baseline, finalBlowDamage, effectedAgent.IsHero, lethalDamage);
    }
}
