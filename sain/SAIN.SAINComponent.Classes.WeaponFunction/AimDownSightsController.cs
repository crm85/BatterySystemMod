using EFT;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Mover;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class AimDownSightsController : BotComponentClassBase
{
	public enum EAimDownSightsStatus
	{
		None,
		HoldInCover,
		StandAndShoot,
		EnemyVisible,
		Sprinting,
		MovingToCover,
		Suppressing,
		DogFight,
		EnemySeenRecent,
		EnemyHeardRecent,
		SearchPeekWait
	}

	public bool AimingDownSights { get; private set; }

	public BotAimingClass BotAimingClass
	{
		get
		{
			IBotAiming currentAiming = base.BotOwner.AimingManager.CurrentAiming;
			return (BotAimingClass)(object)((currentAiming is BotAimingClass) ? currentAiming : null);
		}
	}

	public EAimDownSightsStatus CurrentADSstatus { get; private set; }

	public EAimDownSightsStatus LastADSstatus { get; private set; }

	public AimDownSightsController(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyBotInCombat;
	}

	public override void ManualUpdate()
	{
		SetADS(AimingDownSights);
		base.ManualUpdate();
	}

	public void UpdateADSstatus(Enemy Enemy)
	{
		BotComponent bot = base.Bot;
		if (bot == null || bot.Info?.PersonalitySettings?.Search?.Sneaky != true || Enemy == null || Enemy.IsVisible || !(Enemy.KnownPlaces.EnemyDistanceFromLastKnown < 40f))
		{
			AimingDownSights = ShallAimDownSights(Enemy?.KnownPlaces.LastKnownPosition, Enemy);
		}
	}

	public bool ShallAimDownSights(Vector3? targetPosition = null, Enemy enemy = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		EAimDownSightsStatus eAimDownSightsStatus = EAimDownSightsStatus.None;
		if (targetPosition.HasValue)
		{
			eAimDownSightsStatus = GetADSStatus(targetPosition.Value, enemy);
		}
		float timeSinceChangeDecision = base.Bot.Decision.TimeSinceChangeDecision;
		switch (eAimDownSightsStatus)
		{
		case EAimDownSightsStatus.EnemyVisible:
		case EAimDownSightsStatus.EnemySeenRecent:
		case EAimDownSightsStatus.EnemyHeardRecent:
			result = enemy != null && enemy.KnownPlaces.BotDistanceFromLastKnown > (AimingDownSights ? 10f : 15f);
			break;
		case EAimDownSightsStatus.DogFight:
			result = base.Bot.Mover.DogFight.Status == EDogFightStatus.Shooting;
			break;
		case EAimDownSightsStatus.None:
		case EAimDownSightsStatus.Sprinting:
		case EAimDownSightsStatus.MovingToCover:
			result = false;
			break;
		case EAimDownSightsStatus.HoldInCover:
			result = timeSinceChangeDecision > 3f && (EFTMath.RandomBool(60f) || (enemy != null && enemy.KnownPlaces.BotDistanceFromLastKnown > (AimingDownSights ? 10f : 15f)));
			break;
		case EAimDownSightsStatus.StandAndShoot:
			result = enemy != null && enemy.RealDistance > (AimingDownSights ? 10f : 15f);
			break;
		case EAimDownSightsStatus.Suppressing:
			result = enemy != null && enemy.KnownPlaces.BotDistanceFromLastKnown > (AimingDownSights ? 10f : 15f);
			break;
		}
		LastADSstatus = CurrentADSstatus;
		CurrentADSstatus = eAimDownSightsStatus;
		return result;
	}

	public void SetADS(bool value)
	{
		IBotAiming currentAiming = base.BotOwner.AimingManager.CurrentAiming;
		BotAimingClass val = (BotAimingClass)(object)((currentAiming is BotAimingClass) ? currentAiming : null);
		if (val != null)
		{
			val.HardAim = value;
		}
		IFirearmHandsController shootController = base.BotOwner.WeaponManager.ShootController;
		if (shootController != null && ((IHandsController)shootController).IsAiming != value)
		{
			shootController.SetAim(value);
		}
		AimingDownSights = value;
	}

	public EAimDownSightsStatus GetADSStatus(Vector3 targetPosition, Enemy enemy)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = targetPosition - base.Bot.Position;
		float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
		if (base.Bot.Player.IsSprintEnabled || base.Bot.Mover.PathFollower.Running)
		{
			return EAimDownSightsStatus.Sprinting;
		}
		ECombatDecision currentCombatDecision = base.Bot.Decision.CurrentCombatDecision;
		if (currentCombatDecision == ECombatDecision.ShootDistantEnemy)
		{
			return EAimDownSightsStatus.StandAndShoot;
		}
		if (enemy != null)
		{
			if (enemy.CanShoot && enemy.IsVisible && enemy.RealDistance > 50f)
			{
				return EAimDownSightsStatus.EnemyVisible;
			}
			if (enemy.Seen && enemy.TimeSinceSeen < 5f)
			{
				return EAimDownSightsStatus.EnemySeenRecent;
			}
			if (enemy.Heard && enemy.TimeSinceHeard < 5f)
			{
				return EAimDownSightsStatus.EnemyHeardRecent;
			}
		}
		if (base.Bot.Decision.CurrentSquadDecision == ESquadDecision.Suppress && base.Bot.ManualShoot.Reason == EShootReason.SquadSuppressing)
		{
			return EAimDownSightsStatus.Suppressing;
		}
		switch (currentCombatDecision)
		{
		case ECombatDecision.RunToCover:
		case ECombatDecision.MoveToCover:
			return EAimDownSightsStatus.MovingToCover;
		case ECombatDecision.HoldInCover:
			return EAimDownSightsStatus.HoldInCover;
		case ECombatDecision.StandAndShoot:
			return EAimDownSightsStatus.StandAndShoot;
		case ECombatDecision.DogFight:
			return EAimDownSightsStatus.DogFight;
		case ECombatDecision.Search:
			return (base.Bot.Search.CurrentState != ESearchMove.DirectMove) ? EAimDownSightsStatus.SearchPeekWait : EAimDownSightsStatus.None;
		default:
			return EAimDownSightsStatus.None;
		}
	}
}
