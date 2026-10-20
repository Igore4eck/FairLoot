using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace FairLoot;

/// <summary>
/// Leaving the field without leaving the battle (field battles of the
/// campaign). The game sends the player back to the campaign map, retreating,
/// once he stays past the red line for its 10 s grace time. Half a second
/// before that, while his side still has men fighting, the hero leaves the
/// field like a routing soldier (alive, unhurt, never back on this field) and
/// the battle goes on as after a knockout: the game hands his formations to
/// the AI, the camera turns into the spectator one and the scoreboard (Tab)
/// offers fast forward and ending the battle. With nobody left on his side the
/// game's own retreat happens as before. No patches: only the timing.
/// </summary>
internal sealed class LeaveFieldMissionLogic : MissionLogic
{
    // MissionBoundaryCrossingHandler's grace time is 10 s; act just before it.
    private const float LeaveAfterSeconds = 9.5f;

    private float _outsideSince = -1f;
    private bool _left;

    public override void OnMissionTick(float dt)
    {
        if (_left || !FairLootOptions.LeaveField || Mission == null || Mission.MissionEnded || !Mission.IsFieldBattle)
        {
            return;
        }
        Agent main = Mission.MainAgent;
        if (main == null || !main.IsActive() || Mission.IsPositionInsideBoundaries(main.Position.AsVec2))
        {
            _outsideSince = -1f;
            return;
        }
        float now = Mission.CurrentTime;
        if (_outsideSince < 0f)
        {
            _outsideSince = now;
            return;
        }
        if (now - _outsideSince < LeaveAfterSeconds || !SideFightsOn(main))
        {
            return;
        }
        _left = true;
        main.FadeOut(true, true);
        InformationManager.DisplayMessage(new InformationMessage(
            new TextObject("{=fairloot_left_field}You left the field. The battle goes on under AI command; Tab opens the scoreboard with fast forward and the end of the battle.").ToString(),
            Colors.Yellow));
    }

    // Someone of the player's side, besides himself, still stands on the field.
    private bool SideFightsOn(Agent main)
    {
        foreach (Agent agent in Mission.Agents)
        {
            if (agent != main && agent.IsActive() && agent.IsHuman && !agent.IsRunningAway && agent.Team != null &&
                (agent.Team.IsPlayerTeam || agent.Team.IsPlayerAlly))
            {
                return true;
            }
        }
        return false;
    }
}
