using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace FairLoot;

internal sealed class PersonalLootMissionLogic : MissionLogic
{
    private readonly PersonalLootSession _session;
    private readonly Dictionary<Agent, bool> _lastHitWasSiegeEngine = new();

    internal PersonalLootMissionLogic(PersonalLootSession session) => _session = session;

    public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent,
        WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit,
        in Blow blow, in AttackCollisionData collisionData, float damagedHp,
        float hitDistance, float shotDifficulty)
    {
        if (!isBlocked && affectedAgent != null)
        {
            _session.RecordHit(affectedAgent, blow.InflictedDamage);
        }

        if (!isBlocked && damagedHp > 0f && affectedAgent != null &&
            IsPlayer(affectorAgent))
        {
            _lastHitWasSiegeEngine[affectedAgent] = isSiegeEngineHit;
        }
    }

    public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent,
        AgentState agentState, KillingBlow killingBlow)
    {
        if (affectedAgent == null)
        {
            return;
        }

        bool siegeHit = _lastHitWasSiegeEngine.TryGetValue(affectedAgent, out bool value) && value;
        _lastHitWasSiegeEngine.Remove(affectedAgent);
        _session.ForgetHit(affectedAgent);

        if (agentState != AgentState.Killed || !killingBlow.IsValid ||
            !IsPlayer(affectorAgent) || !affectedAgent.IsHuman ||
            affectedAgent.Team == null || affectorAgent.Team == null ||
            !affectorAgent.Team.IsEnemyOf(affectedAgent.Team) ||
            !IsOnEnemySide(affectedAgent) ||
            siegeHit || IsSiegeProjectile((WeaponClass)killingBlow.WeaponClass))
        {
            return;
        }

        Equipment? equipment = affectedAgent.SpawnEquipment;
        if (equipment == null)
        {
            return;
        }

        for (int slot = 0; slot < 12; slot++)
        {
            EquipmentElement item = equipment[(EquipmentIndex)slot];
            // Items that cannot be traded are skipped, except a harness: a
            // unique barding (a legendary one, a war elephant's royal armor)
            // is taken off its rider like any other. Such mounts stay out.
            if (item.Item != null && (!item.Item.NotMerchandise || item.Item.Type == ItemObject.ItemTypeEnum.HorseHarness) &&
                !item.IsQuestItem && !item.Item.IsBannerItem)
            {
                _session.Add(item);
            }
        }
    }

    public override void OnMissionResultReady(MissionResult missionResult)
    {
        if (missionResult.PlayerDefeated)
        {
            _session.Clear();
        }
    }

    // A retreat keeps the kill loot: it stays bound to this map event and is
    // paid only if the player's side still wins it (autoresolve, his troops).

    private static bool IsPlayer(Agent? agent) =>
        agent != null && agent.IsHuman && agent.Character == Hero.MainHero.CharacterObject;

    // The team check alone let a thrown weapon loot the player's own troops,
    // so the victim's party must also stand on the other side of the map event.
    private static bool IsOnEnemySide(Agent victim)
    {
        MapEventSide? playerSide = PartyBase.MainParty.MapEventSide;
        if (victim.Origin?.BattleCombatant is not PartyBase party || playerSide == null)
        {
            return true;
        }
        MapEventSide? victimSide = party.MapEventSide;
        return victimSide != null && victimSide.MissionSide != playerSide.MissionSide;
    }

    private static bool IsSiegeProjectile(WeaponClass weaponClass) =>
        weaponClass == WeaponClass.BallistaStone ||
        weaponClass == WeaponClass.BallistaBoulder ||
        weaponClass == WeaponClass.Boulder;
}
