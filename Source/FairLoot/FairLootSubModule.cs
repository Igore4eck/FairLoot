using System.Linq;
using SandBox.GameComponents;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace FairLoot;

public sealed class FairLootSubModule : MBSubModuleBase
{
    private readonly PersonalLootSession _session = new();

    protected override void OnBeforeInitialModuleScreenSetAsRoot()
    {
        base.OnBeforeInitialModuleScreenSetAsRoot();
        FairLootOptions.Refresh();
    }

    protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
    {
        base.OnGameStart(game, gameStarterObject);
        FairLootOptions.Refresh();
        _session.Clear();
        if (gameStarterObject is CampaignGameStarter campaignStarter)
        {
            AgentDecideKilledOrUnconsciousModel previousModel =
                campaignStarter.Models.OfType<AgentDecideKilledOrUnconsciousModel>().LastOrDefault()
                ?? new SandboxAgentDecideKilledOrUnconsciousModel();
            campaignStarter.AddModel(new FinalBlowDeathModel(previousModel, _session));
            campaignStarter.AddBehavior(new FairLootCampaignBehavior(_session));
            campaignStarter.AddBehavior(new FairLootWelcomeBehavior());
        }
    }

    public override void OnMissionBehaviorInitialize(Mission mission)
    {
        base.OnMissionBehaviorInitialize(mission);
        // Custom Battle has no campaign: MapEvent.PlayerMapEvent dereferences
        // the campaign's main party and throws there, crashing mission start.
        if (Campaign.Current == null)
        {
            return;
        }
        MapEvent? battle = MapEvent.PlayerMapEvent;
        if (battle != null &&
            mission.CombatType == Mission.MissionCombatType.Combat &&
            mission.GetMissionBehavior<PersonalLootMissionLogic>() == null)
        {
            _session.BeginOrContinue(battle);
            mission.AddMissionBehavior(new PersonalLootMissionLogic(_session));
            mission.AddMissionBehavior(new LeaveFieldMissionLogic());
        }
    }
}
