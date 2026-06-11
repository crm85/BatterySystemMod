using EFT;
using SAIN.Components;
using UnityEngine;

namespace SAIN.BotController.Classes;

public class MemberInfo
{
	private readonly Squad _squad;

	public readonly BotComponent Bot;

	public readonly Player Player;

	public readonly string ProfileId;

	public readonly string Nickname;

	public ETagStatus HealthStatus;

	public bool HasEnemy => Bot?.HasEnemy ?? false;

	public ECombatDecision SoloDecision { get; private set; }

	public ESquadDecision SquadDecision { get; private set; }

	public ESelfDecision SelfDecision { get; private set; }

	public float PowerLevel { get; private set; }

	public MemberInfo(BotComponent sain, Squad squad)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		_squad = squad;
		Bot = sain;
		Player = sain.Player;
		ProfileId = sain.ProfileId;
		Player player = sain.Player;
		object nickname;
		if (player == null)
		{
			nickname = null;
		}
		else
		{
			Profile profile = player.Profile;
			nickname = ((profile != null) ? profile.Nickname : null);
		}
		Nickname = (string)nickname;
		HealthStatus = sain.Memory.Health.HealthStatus;
		sain.Decision.DecisionManager.OnDecisionMade += UpdateDecisions;
		sain.Memory.Health.HealthStatusChanged += UpdateHealth;
		sain.OnDispose += removeMe;
		UpdatePowerLevel();
	}

	private void removeMe()
	{
		_squad?.RemoveMember(ProfileId);
	}

	private void UpdateDecisions(ECombatDecision solo, ESquadDecision squad, ESelfDecision self, BotComponent member)
	{
		SoloDecision = solo;
		SquadDecision = squad;
		SelfDecision = self;
		UpdatePowerLevel();
	}

	public void UpdatePowerLevel()
	{
		BotComponent bot = Bot;
		object obj;
		if (bot == null)
		{
			obj = null;
		}
		else
		{
			Player player = bot.Player;
			obj = ((player != null) ? player.AIData : null);
		}
		IAIData val = (IAIData)obj;
		if (val != null)
		{
			PowerLevel = val.PowerOfEquipment;
		}
	}

	private void UpdateHealth(ETagStatus healthStatus)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		HealthStatus = healthStatus;
	}

	public void Dispose()
	{
		if ((Object)(object)Bot != (Object)null)
		{
			Bot.OnDispose -= removeMe;
			Bot.Decision.DecisionManager.OnDecisionMade -= UpdateDecisions;
			Bot.Memory.Health.HealthStatusChanged -= UpdateHealth;
		}
	}
}
