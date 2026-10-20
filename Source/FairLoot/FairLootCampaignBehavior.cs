using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FairLoot;

internal sealed class FairLootCampaignBehavior : CampaignBehaviorBase
{
    private readonly PersonalLootSession _session;

    internal FairLootCampaignBehavior(PersonalLootSession session) => _session = session;

    public override void RegisterEvents()
    {
        CampaignEvents.OnCollectLootsItemsEvent.AddNonSerializedListener(this, OnCollectLoot);
        CampaignEvents.CanHeroDieEvent.AddNonSerializedListener(this, CanHeroDie);
    }

    public override void SyncData(IDataStore dataStore) { }

    private void CanHeroDie(Hero hero,
        KillCharacterAction.KillCharacterActionDetail causeOfDeath, ref bool result)
    {
        if (causeOfDeath == KillCharacterAction.KillCharacterActionDetail.DiedInBattle &&
            _session.MustAllowBattleDeath(hero))
        {
            result = true;
        }
    }

    private void OnCollectLoot(PartyBase party, ItemRoster loot)
    {
        MapEvent? battle = MapEvent.PlayerMapEvent;
        if (party != PartyBase.MainParty || battle == null)
        {
            return;
        }

        ReduceOrdinaryLoot(loot, FairLootOptions.OrdinaryLootShare);

        IReadOnlyList<EquipmentElement> personalItems = _session.Take(battle);
        if (!FairLootOptions.PersonalLoot)
        {
            return;
        }
        foreach (EquipmentElement item in personalItems)
        {
            loot.AddToCounts(item, 1);
        }

        if (personalItems.Count > 0)
        {
            TextObject message = new("{=fairloot_personal_items}Items received for your personal contribution to the battle: {COUNT}.");
            message.SetTextVariable("COUNT", personalItems.Count);
            InformationManager.DisplayMessage(new InformationMessage(message.ToString(), Colors.Green));
        }
    }

    private static void ReduceOrdinaryLoot(ItemRoster loot, float keptShare)
    {
        var original = new List<ItemRosterElement>();
        int remaining = 0;
        foreach (ItemRosterElement element in loot)
        {
            original.Add(element);
            remaining += element.Amount;
        }

        // Fractional item counts are rounded randomly so the average matches the share.
        float exactToRemove = remaining * (1f - keptShare);
        int toRemove = (int)exactToRemove;
        if (MBRandom.RandomFloat < exactToRemove - toRemove)
        {
            toRemove++;
        }
        if (toRemove <= 0)
        {
            return;
        }

        foreach (ItemRosterElement element in original)
        {
            int removedFromStack = 0;
            for (int i = 0; i < element.Amount; i++)
            {
                if (MBRandom.RandomFloat < (float)toRemove / remaining)
                {
                    removedFromStack++;
                    toRemove--;
                }
                remaining--;
            }

            if (removedFromStack > 0)
            {
                loot.AddToCounts(element.EquipmentElement, -removedFromStack);
            }
        }
    }
}
