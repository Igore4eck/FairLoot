using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace FairLoot;

internal sealed class PersonalLootSession
{
    private readonly List<EquipmentElement> _items = new();
    private readonly Dictionary<Agent, int> _lastHitDamage = new();
    private readonly HashSet<Hero> _heroesKilledByMassiveHit = new();
    private MapEvent? _battle;

    internal void BeginOrContinue(MapEvent battle)
    {
        if (!ReferenceEquals(_battle, battle))
        {
            _items.Clear();
            _lastHitDamage.Clear();
            _heroesKilledByMassiveHit.Clear();
            _battle = battle;
        }
    }

    internal void Add(EquipmentElement item) => _items.Add(item);

    internal void RecordHit(Agent victim, int damage)
    {
        if (damage > 0)
        {
            _lastHitDamage[victim] = damage;
            if (victim.Character is CharacterObject character && character.IsHero)
            {
                Hero hero = character.HeroObject;
                if (FairLootOptions.DeathCurve && damage >= FairLootOptions.LethalDamage &&
                    (FairLootOptions.ApplyToPlayer || hero != Hero.MainHero))
                {
                    _heroesKilledByMassiveHit.Add(hero);
                }
                else
                {
                    _heroesKilledByMassiveHit.Remove(hero);
                }
            }
        }
    }

    internal bool MustAllowBattleDeath(Hero hero) =>
        ReferenceEquals(_battle, MapEvent.PlayerMapEvent) &&
        _heroesKilledByMassiveHit.Contains(hero);

    internal bool TryGetLastHitDamage(Agent victim, out int damage) =>
        _lastHitDamage.TryGetValue(victim, out damage);

    internal void ForgetHit(Agent victim) => _lastHitDamage.Remove(victim);

    internal IReadOnlyList<EquipmentElement> Take(MapEvent battle)
    {
        if (!ReferenceEquals(_battle, battle))
        {
            return System.Array.Empty<EquipmentElement>();
        }

        EquipmentElement[] result = _items.ToArray();
        _items.Clear();
        _lastHitDamage.Clear();
        // Battle deaths are finalized after loot is collected. Keep the hero
        // override until this map event ends or the next battle begins.
        return result;
    }

    internal void Clear()
    {
        _items.Clear();
        _lastHitDamage.Clear();
        _heroesKilledByMassiveHit.Clear();
        _battle = null;
    }
}
