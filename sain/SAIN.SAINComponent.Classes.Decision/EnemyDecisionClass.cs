using System.Text;
using SAIN.Components;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Search;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Decision;

public class EnemyDecisionClass : BotBase
{
	private static readonly float RushEnemyMaxPathDistance = 10f;

	private static readonly float RushEnemyMaxPathDistanceSprint = 20f;

	private static readonly float RushEnemyLowAmmoRatio = 0.5f;

	private static readonly float RunToCoverTime = 1.5f;

	private static readonly float RunToCoverTimeRandomMin = 0.66f;

	private static readonly float RunToCoverTimeRandomMax = 1.33f;

	private const float FREEZE_MAX_DISTANCE = 70f;

	private const float FREEZE_MIN_TIMESINCESEEN = 240f;

	private const float FREEZE_MAX_TIMESINCEHEARD = 80f;

	private float StartRunCoverTimer;

	private float _nextShootDistTargetTime;

	private float _endShootDistTargetTime;

	private float TimeForNewShift;

	private float ShiftResetTimer;

	public SearchReasonsStruct DebugSearchReasons { get; private set; }

	public float FrozenDuration { get; private set; }

	public float TimeToUnfreeze { get; private set; }

	public StringBuilder DecisionReasons { get; } = new StringBuilder();

	public bool ShiftCoverComplete { get; set; }

	public bool? DebugShallSearch { get; set; }

	private CoverSettings CoverSettings => SAINPlugin.LoadedPreset.GlobalSettings.General.Cover;

	private float ShiftCoverChangeDecisionTime => CoverSettings.ShiftCoverChangeDecisionTime;

	private float ShiftCoverTimeSinceSeen => CoverSettings.ShiftCoverTimeSinceSeen;

	private float ShiftCoverTimeSinceEnemyCreated => CoverSettings.ShiftCoverTimeSinceEnemyCreated;

	private float ShiftCoverNoEnemyResetTime => CoverSettings.ShiftCoverNoEnemyResetTime;

	private float ShiftCoverNewCoverTime => CoverSettings.ShiftCoverNewCoverTime;

	private float ShiftCoverResetTime => CoverSettings.ShiftCoverResetTime;

	public EnemyDecisionClass(BotComponent sain)
		: base(sain)
	{
		base.CanEverTick = false;
	}

	public bool GetDecision(out ECombatDecision result, Enemy enemy, EnemyList knownEnemies)
	{
		if (enemy == null)
		{
			result = ECombatDecision.None;
			return false;
		}
		DecisionReasons.Clear();
		BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
		if (weaponManager == null || !weaponManager.HaveBullets || weaponManager.Reload.Reloading)
		{
			result = ECombatDecision.Retreat;
			return true;
		}
		string reason = string.Empty;
		DecisionReasons.AppendLine("1. I've Got Bullets.");
		if (shallDogFight(enemy, out reason))
		{
			result = ECombatDecision.DogFight;
			return true;
		}
		bool flag = true;
		ESuppressionState currentState = base.Bot.Suppression.CurrentState;
		ESuppressionState eSuppressionState = currentState;
		ESuppressionState eSuppressionState2 = eSuppressionState;
		if ((uint)(eSuppressionState2 - 3) <= 1u)
		{
			flag = false;
			reason = $"Suppressed [{currentState}]";
		}
		DecisionReasons.AppendLine($"2. CanTakeAggroActions?: [{flag}, {reason}]");
		if (flag)
		{
			bool flag2 = shallStandAndShoot(enemy, out reason, knownEnemies);
			DecisionReasons.AppendLine($"2. Shall Shoot: [{flag2}, {reason}]");
			if (flag2)
			{
				if (base.Bot.Decision.CurrentCombatDecision != ECombatDecision.StandAndShoot)
				{
					base.Bot.Info.CalcHoldGroundDelay();
				}
				result = ECombatDecision.StandAndShoot;
				return true;
			}
			bool flag3 = shallShootDistantEnemy(enemy, out reason);
			DecisionReasons.AppendLine($"3. Shall Shoot Distant: [{flag3}, {reason}]");
			if (flag3)
			{
				result = ECombatDecision.ShootDistantEnemy;
				return true;
			}
			bool flag4 = shallRushEnemy(enemy, out reason);
			DecisionReasons.AppendLine($"4. Shall Rush: [{flag4}, {reason}]");
			if (flag4)
			{
				result = ECombatDecision.RushEnemy;
				return true;
			}
			bool flag5 = shallThrowGrenade(enemy, out reason);
			DecisionReasons.AppendLine($"5. Shall Throw Nade: [{flag5}, {reason}]");
			if (flag5)
			{
				result = ECombatDecision.ThrowGrenade;
				return true;
			}
			bool flag6 = shallSearch(enemy, out reason);
			DecisionReasons.AppendLine($"6. Shall Search: [{flag6}, {reason}]");
			if (flag6)
			{
				if (base.Bot.Decision.CurrentCombatDecision != ECombatDecision.Search)
				{
					enemy.Status.NumberOfSearchesStarted++;
				}
				result = ECombatDecision.Search;
				return true;
			}
		}
		bool flag7 = shallFreezeAndWait(enemy, out reason);
		DecisionReasons.AppendLine($"7. Shall Freeze: [{flag7}, {reason}]");
		if (flag7)
		{
			result = ECombatDecision.Freeze;
			return true;
		}
		bool flag8 = shallShiftCover(enemy, out reason);
		DecisionReasons.AppendLine($"8. Shall Shift Cover: [{flag8}, {reason}]");
		if (flag8)
		{
			result = ECombatDecision.ShiftCover;
			return true;
		}
		bool flag9 = shallMoveToCover(out reason);
		DecisionReasons.AppendLine($"8. Shall MoveToCover: [{flag9}, {reason}]");
		if (flag9)
		{
			result = ECombatDecision.MoveToCover;
			bool flag10 = shallRunForCover(enemy, out reason);
			DecisionReasons.AppendLine($"8-1. Shall RunToCover: [{flag10}, {reason}]");
			if (flag10)
			{
				result = ECombatDecision.RunToCover;
			}
			return true;
		}
		StartRunCoverTimer = 0f;
		bool flag11 = shallHoldInCover(out reason);
		DecisionReasons.AppendLine($"9. Shall HoldinCover: [{flag11}, {reason}]");
		if (flag11)
		{
			result = ECombatDecision.HoldInCover;
			return true;
		}
		DecisionReasons.AppendLine("10. No Decision?");
		result = ECombatDecision.DebugNoDecision;
		return false;
	}

	private bool shallFreezeAndWait(Enemy enemy, out string reason)
	{
		if (base.Bot.Info.PersonalitySettings.Search.HeardFromPeaceBehavior != EHeardFromPeaceBehavior.Freeze)
		{
			reason = "wontFreeze";
			return false;
		}
		if (!enemy.Hearing.EnemyHeardFromPeace)
		{
			reason = "notHeardFromPeace";
			return false;
		}
		if (!base.Bot.Memory.Location.IsIndoors)
		{
			reason = "outside";
			return false;
		}
		if (enemy.Seen && enemy.TimeSinceSeen < 240f)
		{
			reason = "seenRecent";
			return false;
		}
		if (enemy.TimeSinceLastKnownUpdated > 80f)
		{
			reason = "haventHeard";
			return false;
		}
		if (enemy.KnownPlaces.BotDistanceFromLastKnown > 70f)
		{
			reason = "tooFar";
			return false;
		}
		if (base.Bot.Decision.CurrentCombatDecision != ECombatDecision.Freeze)
		{
			float num = (FrozenDuration = Random.Range(10f, 120f) / base.Bot.Info.AggressionMultiplier);
			TimeToUnfreeze = Time.time + num;
		}
		if (TimeToUnfreeze < Time.time)
		{
			reason = "frozenTooLong";
			return false;
		}
		reason = "timeForFreeze";
		return true;
	}

	private bool shallThrowGrenade(Enemy enemy, out string reason)
	{
		return base.Bot.Grenade.GrenadeThrowDecider.GetDecision(enemy, out reason);
	}

	private bool shallRushEnemy(Enemy enemy, out string reason)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Invalid comparison between Unknown and I4
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Invalid comparison between Unknown and I4
		ETagStatus healthStatus = base.Bot.Memory.Health.HealthStatus;
		if ((int)healthStatus == 8192)
		{
			reason = "imDying";
			return false;
		}
		if ((int)enemy.Path.PathToEnemyStatus > 0)
		{
			reason = "incompletePath";
			return false;
		}
		if (base.Bot.Decision.SelfActionDecisions.LowOnAmmo(RushEnemyLowAmmoRatio))
		{
			reason = "lowAmmo";
			return false;
		}
		if (!checkInRangeForRush(enemy))
		{
			reason = "outOfRange";
			return false;
		}
		if (enemy.Hearing.EnemyHeardFromPeace && base.Bot.Info.PersonalitySettings.Search.HeardFromPeaceBehavior == EHeardFromPeaceBehavior.Charge)
		{
			reason = "heardFromPeaceCharge";
			return true;
		}
		if (!base.Bot.Info.PersonalitySettings.Rush.CanRushEnemyReloadHeal)
		{
			reason = "cantRush";
			return false;
		}
		if (enemy.Status.VulnerableAction != EEnemyAction.None)
		{
			reason = "enemyVulnerable";
			return true;
		}
		ETagStatus healthStatus2 = enemy.EnemyPlayer.HealthStatus;
		if ((int)healthStatus2 == 8192)
		{
			reason = "enemyHurtBad";
			return true;
		}
		if ((int)healthStatus2 == 4096 && enemy.EnemyPlayer.IsInPronePose)
		{
			reason = "enemyHurtAndProne";
			return true;
		}
		reason = "notGoodTimeTo";
		return false;
	}

	private bool checkInRangeForRush(Enemy enemy)
	{
		EEnemyAction vulnerableAction = enemy.Status.VulnerableAction;
		float num = ((vulnerableAction == EEnemyAction.UsingSurgery) ? 2f : 1f);
		if (enemy.Path.PathLength < RushEnemyMaxPathDistance * num)
		{
			return true;
		}
		if (enemy.Path.PathLength < RushEnemyMaxPathDistanceSprint * num && base.BotOwner.CanSprintPlayer)
		{
			return true;
		}
		return false;
	}

	private bool shallShiftCover(Enemy enemy, out string reason)
	{
		if (!base.Bot.Info.PersonalitySettings.Cover.CanShiftCoverPosition)
		{
			reason = "cantShift";
			return false;
		}
		if (base.Bot.Suppression.IsSuppressed)
		{
			reason = "suppressed";
			return false;
		}
		if (ContinueShiftCover())
		{
			reason = "continueShift";
			return true;
		}
		ECombatDecision currentCombatDecision = base.Bot.Decision.CurrentCombatDecision;
		if (currentCombatDecision == ECombatDecision.HoldInCover && base.Bot.Info.PersonalitySettings.Cover.CanShiftCoverPosition && base.Bot.Decision.TimeSinceChangeDecision > ShiftCoverChangeDecisionTime && TimeForNewShift < Time.time)
		{
			if (enemy != null)
			{
				if (enemy.Seen && !enemy.IsVisible && enemy.TimeSinceSeen > ShiftCoverTimeSinceSeen)
				{
					TimeForNewShift = Time.time + ShiftCoverNewCoverTime;
					ShiftResetTimer = Time.time + ShiftCoverResetTime;
					reason = "enemyNotSeen";
					return true;
				}
				if (!enemy.Seen && enemy.KnownPlaces.TimeSinceLastKnownUpdated > ShiftCoverTimeSinceEnemyCreated)
				{
					TimeForNewShift = Time.time + ShiftCoverNewCoverTime;
					ShiftResetTimer = Time.time + ShiftCoverResetTime;
					reason = "lastKnownNotUpdated";
					return true;
				}
			}
			if (enemy == null && base.Bot.Decision.TimeSinceChangeDecision > ShiftCoverNoEnemyResetTime)
			{
				TimeForNewShift = Time.time + ShiftCoverNewCoverTime;
				ShiftResetTimer = Time.time + ShiftCoverResetTime;
				reason = "timeDecisionMade";
				return true;
			}
		}
		reason = "dontWantTo";
		ShiftResetTimer = -1f;
		return false;
	}

	private bool ContinueShiftCover()
	{
		ECombatDecision currentCombatDecision = base.Bot.Decision.CurrentCombatDecision;
		if (currentCombatDecision == ECombatDecision.ShiftCover)
		{
			if (ShiftResetTimer > 0f && ShiftResetTimer < Time.time)
			{
				ShiftResetTimer = -1f;
				return false;
			}
			if (!base.Bot.Mover.Moving)
			{
				return false;
			}
			if (!ShiftCoverComplete)
			{
				return true;
			}
		}
		return false;
	}

	private bool shallDogFight(Enemy enemy, out string reason)
	{
		if (base.Bot.Decision.CurrentSelfDecision != ESelfDecision.None || base.BotOwner.WeaponManager.Reload.Reloading)
		{
			reason = "selfDecisionOrReloading";
			return false;
		}
		if (base.Bot.Decision.CurrentCombatDecision == ECombatDecision.RushEnemy)
		{
			reason = "rushingEnemy";
			return false;
		}
		if (base.Bot.Cover.SpottedInCover)
		{
			reason = "coverSpotted";
			return true;
		}
		if (enemy != null && enemy.EPathDistance == EPathDistance.VeryClose && enemy.IsVisible)
		{
			reason = "enemyClose";
			return true;
		}
		reason = string.Empty;
		return false;
	}

	private bool shallMoveToEngage(Enemy enemy)
	{
		if (base.Bot.Suppression.IsSuppressed)
		{
			return false;
		}
		if (!enemy.Seen || enemy.TimeSinceSeen < 8f)
		{
			return false;
		}
		if (enemy.IsVisible && enemy.EnemyLookingAtMe)
		{
			return false;
		}
		ECombatDecision currentCombatDecision = base.Bot.Decision.CurrentCombatDecision;
		if (base.BotOwner.Memory.IsUnderFire && currentCombatDecision != ECombatDecision.MoveToEngage)
		{
			return false;
		}
		if (currentCombatDecision == ECombatDecision.Retreat || currentCombatDecision == ECombatDecision.MoveToCover || currentCombatDecision == ECombatDecision.RunToCover)
		{
			return false;
		}
		if (enemy.RealDistance > base.Bot.Info.WeaponInfo.EffectiveWeaponDistance && currentCombatDecision != ECombatDecision.MoveToEngage)
		{
			return true;
		}
		if (enemy.RealDistance > base.Bot.Info.WeaponInfo.EffectiveWeaponDistance * 0.66f && currentCombatDecision == ECombatDecision.MoveToEngage)
		{
			return true;
		}
		return false;
	}

	private bool shallShootDistantEnemy(Enemy enemy, out string reason)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Invalid comparison between Unknown and I4
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Invalid comparison between Unknown and I4
		if (_endShootDistTargetTime > Time.time && base.Bot.Decision.CurrentCombatDecision == ECombatDecision.ShootDistantEnemy && (int)base.Bot.Memory.Health.HealthStatus != 8192)
		{
			reason = "shootingDistantEnemy";
			return true;
		}
		if (_nextShootDistTargetTime < Time.time && enemy.RealDistance > base.Bot.Info.FileSettings.Shoot.MaxPointFireDistance && enemy.IsVisible && enemy.CanShoot && ((int)base.Bot.Memory.Health.HealthStatus == 1024 || (int)base.Bot.Memory.Health.HealthStatus == 2048))
		{
			float num = 6f * Random.Range(0.75f, 1.25f);
			_nextShootDistTargetTime = Time.time + num;
			_endShootDistTargetTime = Time.time + num / 3f;
			reason = "shootingDistantEnemy";
			return true;
		}
		reason = string.Empty;
		return false;
	}

	private bool shallRunForCover(Enemy enemy, out string reason)
	{
		if (!base.BotOwner.CanSprintPlayer)
		{
			reason = "cantSprint";
			return false;
		}
		if (base.Bot.Cover.CoverPoints.Count == 0)
		{
			reason = "noCoverPoints";
			return false;
		}
		if (enemy.IsVisible || !enemy.Seen || enemy.TimeSinceSeen > 3f)
		{
		}
		if (enemy.IsSniper && BotBase.GlobalSettings.Mind.ENEMYSNIPER_ALWAYS_SPRINT_COVER)
		{
			reason = "EnemySniperRun";
			return true;
		}
		if (StartRunCoverTimer < Time.time)
		{
			reason = "timeToRun";
			return true;
		}
		reason = "dontRunYet";
		return false;
	}

	private bool shallMoveToCover(out string reason)
	{
		if (base.Bot.Cover.InCover)
		{
			reason = "inCover";
			return false;
		}
		ECombatDecision currentCombatDecision = base.Bot.Decision.CurrentCombatDecision;
		if (currentCombatDecision != ECombatDecision.MoveToCover && currentCombatDecision != ECombatDecision.RunToCover)
		{
			StartRunCoverTimer = Time.time + RunToCoverTime * Random.Range(RunToCoverTimeRandomMin, RunToCoverTimeRandomMax);
		}
		reason = "notInCover";
		return true;
	}

	private bool shallSearch(Enemy enemy, out string reason)
	{
		SearchReasonsStruct reasons;
		bool flag = base.Bot.Search.SearchDecider.ShallStartSearch(enemy, out reasons);
		DebugSearchReasons = reasons;
		DebugShallSearch = flag;
		if (flag)
		{
			reason = "wantToSearch";
		}
		else
		{
			reason = "cantSearch";
		}
		return flag;
	}

	public bool shallHoldInCover(out string reason)
	{
		if (base.Bot.Cover.InCover)
		{
			reason = "inCover";
			return true;
		}
		reason = "notInCover";
		return false;
	}

	private bool shallStandAndShoot(Enemy enemy, out string reason, EnemyList KnownEnemies)
	{
		if (!enemy.IsVisible)
		{
			reason = "cantSeeEnemy";
			return false;
		}
		if (!enemy.CanShoot)
		{
			reason = "cantShootEnemy";
			return false;
		}
		BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
		if (weaponManager != null && !weaponManager.HaveBullets)
		{
			reason = "noBullets";
			return false;
		}
		if (enemy.RealDistance > base.Bot.Info.WeaponInfo.EffectiveWeaponDistance * 1.25f)
		{
			reason = "outOfRange";
			return false;
		}
		if (enemy.IsZombie)
		{
			bool flag = false;
			foreach (Enemy KnownEnemy in KnownEnemies)
			{
				if (KnownEnemy == null || !KnownEnemy.IsZombie)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				reason = "shootZombie";
				return true;
			}
		}
		bool value = enemy.Events.OnSearch.Value;
		float num = base.Bot.Info.HoldGroundDelay;
		if (value)
		{
			num = Mathf.Max(num, 0.5f) * Random.Range(0.66f, 1.33f);
		}
		if (num <= 0f)
		{
			reason = "wontHoldGround";
			return false;
		}
		if (!enemy.EnemyLookingAtMe)
		{
			reason = "enemyNotLooking";
			return true;
		}
		float num2 = Time.time - enemy.Vision.VisibleStartTime;
		if (num2 > num)
		{
			reason = "visibleTooLong";
			return false;
		}
		if (num2 < num / 1.5f)
		{
			reason = "holdingFromTime";
			return true;
		}
		if (base.Bot.Cover.CheckLimbsForCover())
		{
			reason = "holdingHaveSomeCover";
			return true;
		}
		reason = "outOfTime";
		return false;
	}
}
