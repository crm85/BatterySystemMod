using SAIN.BotController.Classes;
using SAIN.Models.Enums;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class SteerPriorityClass : BotSubClass<SAINSteeringClass>
{
	private readonly float Steer_TimeSinceLocationKnown_Threshold = 3f;

	private readonly float Steer_TimeSinceSeen_Long = 60f;

	private readonly float Steer_HeardSound_Dist = 50f;

	private readonly float Steer_HeardSound_Age = 3f;

	public ESteerPriority CurrentSteerPriority { get; private set; }

	public ESteerPriority LastSteerPriority { get; private set; }

	public PlaceForCheck LastHeardSound { get; private set; }

	public Enemy EnemyWhoLastShotMe { get; private set; }

	public AimStatus AimStatus
	{
		get
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Invalid comparison between Unknown and I4
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			if (base.BotOwner.AimingManager.CurrentAiming != null)
			{
				IBotAiming currentAiming = base.BotOwner.AimingManager.CurrentAiming;
				BotAimingClass val = (BotAimingClass)(object)((currentAiming is BotAimingClass) ? currentAiming : null);
				if (val != null)
				{
					AimStatus aimStatus_ = val.aimStatus_0;
					if ((int)aimStatus_ != 1)
					{
						Enemy enemy = base.Bot.Enemy;
						if (enemy != null && !enemy.IsVisible)
						{
							Enemy lastEnemy = base.Bot.LastEnemy;
							if (lastEnemy != null && !lastEnemy.IsVisible)
							{
								return (AimStatus)1;
							}
						}
					}
					return aimStatus_;
				}
			}
			return (AimStatus)1;
		}
	}

	public SteerPriorityClass(SAINSteeringClass steering)
		: base(steering)
	{
	}

	public ESteerPriority GetCurrentSteerPriority(bool lookRandom, bool ignoreRunningPath)
	{
		ESteerPriority currentSteerPriority = CurrentSteerPriority;
		CurrentSteerPriority = findSteerPriority(lookRandom, ignoreRunningPath);
		if (CurrentSteerPriority != currentSteerPriority)
		{
			LastSteerPriority = currentSteerPriority;
		}
		return CurrentSteerPriority;
	}

	private ESteerPriority findSteerPriority(bool lookRandom, bool ignoreRunningPath)
	{
		ESteerPriority eSteerPriority = strickChecks(ignoreRunningPath);
		if (eSteerPriority != ESteerPriority.None)
		{
			return eSteerPriority;
		}
		eSteerPriority = reactiveSteering();
		if (eSteerPriority != ESteerPriority.None)
		{
			return eSteerPriority;
		}
		eSteerPriority = senseSteering();
		if (eSteerPriority != ESteerPriority.None)
		{
			return eSteerPriority;
		}
		if (lookRandom)
		{
			return ESteerPriority.RandomLook;
		}
		return ESteerPriority.None;
	}

	private ESteerPriority strickChecks(bool ignoreRunningPath)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (!ignoreRunningPath && base.Bot.Mover.PathFollower.Running)
		{
			return ESteerPriority.RunningPath;
		}
		if (base.Player.IsSprintEnabled)
		{
			return ESteerPriority.Sprinting;
		}
		if (lookToAimTarget())
		{
			return ESteerPriority.Aiming;
		}
		if (base.Bot.ManualShoot.Reason != EShootReason.None && base.Bot.ManualShoot.ShootPosition != Vector3.zero)
		{
			return ESteerPriority.ManualShooting;
		}
		if (enemyVisible())
		{
			return ESteerPriority.EnemyVisible;
		}
		return ESteerPriority.None;
	}

	private ESteerPriority reactiveSteering()
	{
		if (enemyShotMe())
		{
			return ESteerPriority.LastHit;
		}
		if (base.BotOwner.Memory.IsUnderFire)
		{
			return ESteerPriority.UnderFire;
		}
		return ESteerPriority.None;
	}

	private ESteerPriority senseSteering()
	{
		EnemyPlace enemyPlace = base.Bot.Enemy?.KnownPlaces?.LastKnownPlace;
		if (enemyPlace != null && enemyPlace.TimeSincePositionUpdated < Steer_TimeSinceLocationKnown_Threshold)
		{
			return ESteerPriority.EnemyLastKnown;
		}
		if (heardThreat())
		{
			return ESteerPriority.HeardThreat;
		}
		if (enemyPlace != null && enemyPlace.TimeSincePositionUpdated < Steer_TimeSinceSeen_Long)
		{
			return ESteerPriority.EnemyLastKnownLong;
		}
		return ESteerPriority.None;
	}

	private bool heardThreat()
	{
		SoundStruct? lastHeardVisibleDanger = base.BaseClass.HeardSoundSteering.LastHeardVisibleDanger;
		if (lastHeardVisibleDanger.HasValue && lastHeardVisibleDanger.GetValueOrDefault().ShallLook)
		{
			return true;
		}
		if (base.Bot.Search.SearchActive)
		{
			return false;
		}
		lastHeardVisibleDanger = base.BaseClass.HeardSoundSteering.LastHeardDanger;
		if (lastHeardVisibleDanger.HasValue && lastHeardVisibleDanger.GetValueOrDefault().ShallLook)
		{
			return true;
		}
		return false;
	}

	private bool heardThreat(out PlaceForCheck placeForCheck)
	{
		placeForCheck = base.BotOwner.BotsGroup.YoungestFastPlace(base.BotOwner, Steer_HeardSound_Dist, Steer_HeardSound_Age);
		if (placeForCheck != null)
		{
			Enemy enemy = base.Bot.Enemy;
			if (enemy == null)
			{
				return true;
			}
			Squad squadInfo = base.Bot.Squad.SquadInfo;
			if (squadInfo != null && squadInfo.PlayerPlaceChecks.TryGetValue(enemy.EnemyProfileId, out var value) && value != placeForCheck)
			{
				return true;
			}
		}
		return false;
	}

	private bool enemyShotMe()
	{
		float timeSinceShot = base.Bot.Medical.TimeSinceShot;
		if (timeSinceShot > 3f || timeSinceShot < 0.2f)
		{
			EnemyWhoLastShotMe = null;
			return false;
		}
		Enemy enemyWhoLastShotMe = base.Bot.Medical.HitByEnemy.EnemyWhoLastShotMe;
		if (enemyWhoLastShotMe != null && enemyWhoLastShotMe.CheckValid() && enemyWhoLastShotMe.EnemyPerson.Active && !enemyWhoLastShotMe.IsCurrentEnemy)
		{
			EnemyWhoLastShotMe = enemyWhoLastShotMe;
			return true;
		}
		EnemyWhoLastShotMe = null;
		return false;
	}

	private bool lookToAimTarget()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		if ((int)base.Bot.Aim.AimStatus == 1)
		{
			return false;
		}
		return canSeeAndShoot(base.Bot.Enemy) || canSeeAndShoot(base.Bot.LastEnemy) || canSeeAndShoot(base.Bot.Shoot.LastShotEnemy);
	}

	private bool canSeeAndShoot(Enemy enemy)
	{
		return enemy != null && enemy.IsVisible && enemy.CanShoot;
	}

	private bool enemyVisible()
	{
		Enemy enemy = base.Bot.Enemy;
		if (enemy != null)
		{
			if (enemy.IsVisible)
			{
				return true;
			}
			if (enemy.Seen && enemy.TimeSinceSeen < 0.5f)
			{
				return true;
			}
		}
		return false;
	}
}
