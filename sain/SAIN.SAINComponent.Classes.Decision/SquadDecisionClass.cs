using EFT;
using SAIN.Components;
using SAIN.Models.Enums;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Decision;

public class SquadDecisionClass : BotBase
{
	private float SquaDecision_RadioCom_MaxDistSq = 1200f;

	private float SquadDecision_MyEnemySeenRecentTime = 10f;

	private static readonly float PushSuppressedEnemyMaxPathDistance = 75f;

	private static readonly float PushSuppressedEnemyMaxPathDistanceSprint = 100f;

	private static readonly float PushSuppressedEnemyLowAmmoRatio = 0.5f;

	private float SquadDecision_SuppressFriendlyDistStart = 30f;

	private float SquadDecision_SuppressFriendlyDistEnd = 50f;

	private float SquadDecision_StartHelpFriendDist = 30f;

	private float SquadDecision_EndHelpFriendDist = 45f;

	private float SquadDecision_EndHelp_FriendsEnemySeenRecentTime = 8f;

	private float SquadDecision_Regroup_NoEnemy_StartDist = 125f;

	private float SquadDecision_Regroup_NoEnemy_EndDistance = 50f;

	private float SquadDecision_Regroup_Enemy_StartDist = 50f;

	private float SquadDecision_Regroup_Enemy_EndDistance = 15f;

	private float SquadDecision_Regroup_EnemySeenRecentTime = 60f;

	private SAINSquadClass Squad => base.Bot.Squad;

	private bool HasRadioComms => base.Bot.PlayerComponent.Equipment.GearInfo.HasEarPiece;

	public SquadDecisionClass(BotComponent sain)
		: base(sain)
	{
		base.CanEverTick = false;
	}

	public bool GetDecision(out ESquadDecision Decision, Enemy enemy)
	{
		Decision = ESquadDecision.None;
		if (Squad.BotInGroup && !((Object)(object)base.Bot.Squad.SquadInfo?.LeaderComponent == (Object)null))
		{
			BotComponent leaderComponent = Squad.LeaderComponent;
			if (leaderComponent == null || !leaderComponent.IsDead)
			{
				if (EnemyDecision(out Decision, enemy))
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}

	private bool EnemyDecision(out ESquadDecision Decision, Enemy enemy)
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		Decision = ESquadDecision.None;
		Enemy enemy2 = base.Bot.Enemy;
		if (shallPushSuppressedEnemy(enemy2))
		{
			Decision = ESquadDecision.PushSuppressedEnemy;
			return true;
		}
		if (enemy2 != null && (enemy2.IsVisible || enemy2.TimeSinceSeen < SquadDecision_MyEnemySeenRecentTime))
		{
			return false;
		}
		if ((Object)(object)base.Bot.Squad.LeaderComponent != (Object)null && shallGroupSearch())
		{
			Decision = ESquadDecision.GroupSearch;
			return true;
		}
		foreach (BotComponent value in base.Bot.Squad.Members.Values)
		{
			if ((Object)(object)value == (Object)null || (Object)(object)value.BotOwner == (Object)(object)base.BotOwner || value.BotOwner.IsDead)
			{
				continue;
			}
			if (!HasRadioComms)
			{
				Vector3 val = base.Bot.Transform.Position - value.Transform.Position;
				if (((Vector3)(ref val)).sqrMagnitude > SquaDecision_RadioCom_MaxDistSq)
				{
					continue;
				}
			}
			if (enemy2 != null && value.HasEnemy && enemy2.EnemyIPlayer == value.Enemy.EnemyIPlayer)
			{
				if (shallSuppressEnemy(value))
				{
					Decision = ESquadDecision.Suppress;
					return true;
				}
				if (shallHelp(value))
				{
					Decision = ESquadDecision.Help;
					return true;
				}
			}
		}
		return false;
	}

	private bool shallPushSuppressedEnemy(Enemy enemy)
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Invalid comparison between Unknown and I4
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Invalid comparison between Unknown and I4
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Invalid comparison between Unknown and I4
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Invalid comparison between Unknown and I4
		bool flag;
		if (enemy != null && !base.Bot.Decision.SelfActionDecisions.LowOnAmmo(PushSuppressedEnemyLowAmmoRatio) && base.Bot.Info.PersonalitySettings.Rush.CanRushEnemyReloadHeal)
		{
			flag = false;
			float num = ((enemy.Status.VulnerableAction == EEnemyAction.UsingSurgery) ? 1.25f : 1f);
			if (enemy.Path.PathLength < PushSuppressedEnemyMaxPathDistanceSprint * num)
			{
				BotOwner botOwner = base.BotOwner;
				if (botOwner != null && botOwner.CanSprintPlayer)
				{
					flag = true;
					goto IL_00b5;
				}
			}
			if (enemy.Path.PathLength < PushSuppressedEnemyMaxPathDistance * num)
			{
				flag = true;
			}
			goto IL_00b5;
		}
		goto IL_0193;
		IL_00b5:
		if (flag && ((int)base.Bot.Memory.Health.HealthStatus == 1024 || (int)base.Bot.Memory.Health.HealthStatus == 2048) && base.Bot.Squad.SquadInfo.SquadIsSuppressEnemy(enemy.EnemyPlayer.ProfileId, out var suppressingMember) && (Object)(object)suppressingMember != (Object)(object)base.Bot)
		{
			SAINEnemyStatus status = enemy.Status;
			if (enemy.Status.VulnerableAction != EEnemyAction.None)
			{
				return true;
			}
			ETagStatus healthStatus = enemy.EnemyPlayer.HealthStatus;
			if ((int)healthStatus == 8192 || (int)healthStatus == 4096)
			{
				return true;
			}
			if (enemy.EnemyPlayer.IsInPronePose)
			{
				return true;
			}
		}
		goto IL_0193;
		IL_0193:
		return false;
	}

	private bool shallSuppressEnemy(BotComponent member)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = base.Bot.Enemy;
		if (enemy == null || !enemy.SuppressionTarget.HasValue)
		{
			return false;
		}
		Enemy enemy2 = base.Bot.Enemy;
		if (enemy2 != null && enemy2.IsVisible)
		{
			return false;
		}
		if (member.Decision.CurrentCombatDecision != ECombatDecision.Retreat)
		{
			return false;
		}
		Vector3 val = member.Transform.Position - base.BotOwner.Position;
		float magnitude = ((Vector3)(ref val)).magnitude;
		float ammoRatio = base.Bot.Decision.SelfActionDecisions.AmmoRatio;
		if (base.Bot.Decision.CurrentSquadDecision == ESquadDecision.Suppress)
		{
			return magnitude <= SquadDecision_SuppressFriendlyDistEnd && ammoRatio >= 0.1f;
		}
		return magnitude <= SquadDecision_SuppressFriendlyDistStart && ammoRatio >= 0.5f;
	}

	private bool shallGroupSearch(BotComponent member)
	{
		if (member.Decision.CurrentCombatDecision == ECombatDecision.Search || member.Decision.CurrentSquadDecision == ESquadDecision.Search)
		{
			return true;
		}
		return false;
	}

	private bool shallGroupSearch()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Bot.Info.Profile.IsBoss || (int)base.Bot.Info.Profile.WildSpawnType != 26)
		{
		}
		foreach (BotComponent value in base.Bot.Squad.Members.Values)
		{
			if (value.Decision.CurrentCombatDecision == ECombatDecision.Search)
			{
				if (base.Bot.Enemy != null && doesMemberShareEnemy(value))
				{
					return true;
				}
				if (base.Bot.Enemy == null && base.Bot.CurrentTargetPosition.HasValue && doesMemberShareTarget(value, base.Bot.CurrentTargetPosition.Value))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool doesMemberShareTarget(BotComponent member, Vector3 targetPosition, float maxDist = 20f)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)member == (Object)null) && !(member.ProfileId == base.Bot.ProfileId))
		{
			BotOwner botOwner = member.BotOwner;
			if (botOwner == null || !botOwner.IsDead)
			{
				int result;
				if (member.CurrentTargetPosition.HasValue)
				{
					Vector3 val = member.CurrentTargetPosition.Value - targetPosition;
					result = ((((Vector3)(ref val)).sqrMagnitude < maxDist) ? 1 : 0);
				}
				else
				{
					result = 0;
				}
				return (byte)result != 0;
			}
		}
		return false;
	}

	private bool doesMemberShareEnemy(BotComponent member)
	{
		if (!((Object)(object)member == (Object)null) && !(member.ProfileId == base.Bot.ProfileId))
		{
			BotOwner botOwner = member.BotOwner;
			if (botOwner == null || !botOwner.IsDead)
			{
				return member.Enemy != null && member.Enemy.EnemyPlayer.ProfileId == base.Bot.Enemy.EnemyPlayer.ProfileId;
			}
		}
		return false;
	}

	private bool shallHelp(BotComponent member)
	{
		float pathLength = member.Enemy.Path.PathLength;
		bool isVisible = member.Enemy.IsVisible;
		if (base.Bot.Decision.CurrentSquadDecision == ESquadDecision.Help && member.Enemy.Seen)
		{
			return pathLength < SquadDecision_EndHelpFriendDist && member.Enemy.TimeSinceSeen < SquadDecision_EndHelp_FriendsEnemySeenRecentTime;
		}
		return pathLength < SquadDecision_StartHelpFriendDist && isVisible;
	}

	public bool shallRegroup()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		SAINSquadClass squad = base.Bot.Squad;
		if (squad.IAmLeader)
		{
			return false;
		}
		float num = SquadDecision_Regroup_NoEnemy_StartDist;
		float num2 = SquadDecision_Regroup_NoEnemy_EndDistance;
		Enemy enemy = base.Bot.Enemy;
		if (enemy != null)
		{
			if (enemy.IsVisible || (enemy.Seen && enemy.TimeSinceSeen < SquadDecision_Regroup_EnemySeenRecentTime))
			{
				return false;
			}
			num = SquadDecision_Regroup_Enemy_StartDist;
			num2 = SquadDecision_Regroup_Enemy_EndDistance;
		}
		BotComponent leaderComponent = squad.LeaderComponent;
		if ((Object)(object)leaderComponent != (Object)null)
		{
			Vector3 position = base.BotOwner.Position;
			Vector3 position2 = leaderComponent.Transform.Position;
			Vector3 val = position2 - position;
			float magnitude = ((Vector3)(ref val)).magnitude;
			if (enemy != null)
			{
				Vector3 enemyPosition = enemy.EnemyPosition;
				Vector3 val2 = enemyPosition - position;
				float magnitude2 = ((Vector3)(ref val2)).magnitude;
				if (magnitude2 < magnitude && magnitude2 < 30f && Vector3.Dot(((Vector3)(ref val2)).normalized, ((Vector3)(ref val)).normalized) > 0.25f)
				{
					return false;
				}
			}
			if (base.Bot.Decision.CurrentSquadDecision == ESquadDecision.Regroup)
			{
				return magnitude > num2;
			}
			return magnitude > num;
		}
		return false;
	}
}
