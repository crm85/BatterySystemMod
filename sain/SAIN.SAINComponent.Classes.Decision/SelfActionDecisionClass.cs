using System.Collections.Generic;
using EFT;
using EFT.InventoryLogic;
using SAIN.Components;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Decision;

public class SelfActionDecisionClass : BotBase
{
	private float _lastReloadTime;

	private float _nextCheckHealTime;

	private float _ammoRatio;

	private float _nextGetRatioTime;

	public ESelfDecision CurrentSelfAction => base.Bot.Decision.CurrentSelfDecision;

	private float _timeSinceChangeDecision => Time.time - base.Bot.Decision.ChangeDecisionTime;

	public bool UsingMeds
	{
		get
		{
			BotMedecine medecine = base.BotOwner.Medecine;
			return medecine != null && medecine.Using && CurrentSelfAction != ESelfDecision.None;
		}
	}

	public bool CanUseStims
	{
		get
		{
			GClass475 val = base.BotOwner.Medecine?.Stimulators;
			return val != null && val.HaveSmt && Time.time - val.LastEndUseTime > 3f && val != null && val.CanUseNow() && !base.Bot.Memory.Health.Healthy;
		}
	}

	public bool CanUseFirstAid
	{
		get
		{
			BotMedecine medecine = base.BotOwner.Medecine;
			int result;
			if (medecine == null)
			{
				result = 0;
			}
			else
			{
				BotFirstAidClass firstAid = medecine.FirstAid;
				result = ((((firstAid != null) ? new bool?(firstAid.ShallStartUse()) : ((bool?)null)) == true) ? 1 : 0);
			}
			return (byte)result != 0;
		}
	}

	public bool CanUseSurgery
	{
		get
		{
			BotMedecine medecine = base.BotOwner.Medecine;
			int result;
			if (medecine != null)
			{
				GClass473 surgicalKit = medecine.SurgicalKit;
				if (((surgicalKit != null) ? new bool?(surgicalKit.ShallStartUse()) : ((bool?)null)) == true)
				{
					BotMedecine medecine2 = base.BotOwner.Medecine;
					if (medecine2 == null)
					{
						result = 0;
					}
					else
					{
						BotFirstAidClass firstAid = medecine2.FirstAid;
						result = ((((firstAid != null) ? new bool?(firstAid.IsBleeding) : ((bool?)null)) == false) ? 1 : 0);
					}
					goto IL_0082;
				}
			}
			result = 0;
			goto IL_0082;
			IL_0082:
			return (byte)result != 0;
		}
	}

	public bool CanReload
	{
		get
		{
			BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
			int result;
			if (weaponManager != null && weaponManager.IsReady)
			{
				BotWeaponManager weaponManager2 = base.BotOwner.WeaponManager;
				result = ((weaponManager2 != null && weaponManager2.Reload.CanReload(false)) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			return (byte)result != 0;
		}
	}

	public float AmmoRatio
	{
		get
		{
			if (_nextGetRatioTime < Time.time)
			{
				_nextGetRatioTime = Time.time + 0.025f;
				BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
				_ammoRatio = getAmmoRatio((weaponManager != null) ? weaponManager.Reload : null);
			}
			return _ammoRatio;
		}
	}

	public SelfActionDecisionClass(BotComponent sain)
		: base(sain)
	{
		base.CanEverTick = false;
	}

	public bool GetDecision(out ESelfDecision Decision, Enemy enemy)
	{
		if (enemy == null)
		{
			Decision = ESelfDecision.None;
			return false;
		}
		BotOwner botOwner = base.BotOwner;
		BotWeaponManager weaponManager = botOwner.WeaponManager;
		if (weaponManager != null && weaponManager.Reload.Reloading)
		{
			_lastReloadTime = Time.time;
		}
		if (CheckContinueSelfAction(out Decision, enemy))
		{
			return true;
		}
		if (botOwner.ShootData.Shooting)
		{
			Decision = ESelfDecision.None;
			return false;
		}
		if (CheckDoReload(enemy, base.Bot))
		{
			botOwner.ShootData.BlockFor(0.7f);
			Decision = ESelfDecision.Reload;
			return true;
		}
		return StartBotHeal(ref Decision);
	}

	private bool CheckDoReload(Enemy enemy, BotComponent bot)
	{
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Invalid comparison between Unknown and I4
		if (Time.time - _lastReloadTime < 1f)
		{
			return false;
		}
		BotOwner botOwner = bot.BotOwner;
		BotMedecine medecine = botOwner.Medecine;
		if (medecine != null && medecine.Using)
		{
			return false;
		}
		AbstractHandsController handsController = botOwner.GetPlayer.HandsController;
		if (handsController != null && handsController.IsInInteractionStrictCheck())
		{
			return false;
		}
		BotWeaponManager weaponManager = botOwner.WeaponManager;
		if (weaponManager == null)
		{
			return false;
		}
		if (weaponManager.IsMelee)
		{
			if (!weaponManager.Melee.ShallEndRun)
			{
				return false;
			}
			BotReload reload = weaponManager.Reload;
			if (reload == null || !reload.Reloading)
			{
				weaponManager.Selector.TryChangeWeapon(true);
			}
			return false;
		}
		BotReload reload2 = weaponManager.Reload;
		if (reload2 == null)
		{
			return false;
		}
		if (reload2.Reloading)
		{
			return false;
		}
		if (!weaponManager.IsReady)
		{
			return false;
		}
		IFirearmHandsController shootController = weaponManager.ShootController;
		if (shootController == null || !shootController.CanStartReload())
		{
			return false;
		}
		if (botOwner.WeaponManager.Malfunctions.HaveMalfunction() && (int)botOwner.WeaponManager.Malfunctions.MalfunctionType() != 1)
		{
			return false;
		}
		_nextGetRatioTime = Time.time + 0.025f;
		_ammoRatio = getAmmoRatio(reload2);
		if (_ammoRatio <= 0f)
		{
			botOwner.ShootData.EndShoot();
		}
		if (CheckReloadRatiosCanReload(enemy, 0.7f, 0.8f, _ammoRatio))
		{
			MagazineItemClass val = default(MagazineItemClass);
			List<AmmoItemClass> list = default(List<AmmoItemClass>);
			if (reload2.CanReload(true, ref val, ref list))
			{
				botOwner.ShootData.EndShoot();
				if (val != null)
				{
					reload2.ReloadMagazine(val);
					reload2.Reloading = true;
					_lastReloadTime = Time.time;
					return true;
				}
				if (list != null && list.Count > 0)
				{
					reload2.ReloadAmmo(list);
					reload2.Reloading = true;
					_lastReloadTime = Time.time;
					return true;
				}
			}
			if (enemy != null && enemy.IsVisible && enemy.RealDistance < 10f && !weaponManager.Selector.TryChangeWeapon(true) && weaponManager.Selector.CanChangeToMeleeWeapons)
			{
				weaponManager.Selector.ChangeToMelee();
			}
		}
		return false;
	}

	private static bool CheckReloadRatiosCanReload(Enemy enemy, float RELOAD_AMMORATIO_MIN_PEACE, float RELOAD_AMMORATIO_MAX, float ammoRatio)
	{
		if (ammoRatio > 0f)
		{
			if (ammoRatio >= RELOAD_AMMORATIO_MAX)
			{
				return false;
			}
			if (enemy == null)
			{
				if (ammoRatio < RELOAD_AMMORATIO_MIN_PEACE)
				{
					return true;
				}
				return false;
			}
			if (!CheckReloadByAmmoRemaining(enemy, ammoRatio))
			{
				return false;
			}
		}
		return true;
	}

	private bool StartBotHeal(ref ESelfDecision Decision)
	{
		if (_nextCheckHealTime < Time.time)
		{
			float timeSinceShot = base.Bot.Medical.TimeSinceShot;
			if (timeSinceShot < 0.5f)
			{
				_nextCheckHealTime = Time.time + 0.2f;
				return false;
			}
			_nextCheckHealTime = Time.time + 1f;
			if (startUseStims())
			{
				Decision = ESelfDecision.Stims;
				return true;
			}
			if (startFirstAid())
			{
				Decision = ESelfDecision.FirstAid;
				return true;
			}
			if (base.Bot.Medical.Surgery.AreaClearForSurgery)
			{
				Decision = ESelfDecision.Surgery;
				return true;
			}
		}
		return false;
	}

	private bool CheckContinueSelfAction(out ESelfDecision Decision, Enemy enemy)
	{
		Decision = ESelfDecision.None;
		switch (CurrentSelfAction)
		{
		case ESelfDecision.FirstAid:
			return checkContinueFirstAid(_timeSinceChangeDecision, out Decision, enemy);
		case ESelfDecision.Reload:
			if (checkContinueReload(_timeSinceChangeDecision, enemy))
			{
				Decision = ESelfDecision.Reload;
				return true;
			}
			return false;
		case ESelfDecision.Surgery:
			return checkContinueSurgery(out Decision);
		case ESelfDecision.Stims:
			return checkContinueStims(_timeSinceChangeDecision, out Decision);
		default:
			Decision = ESelfDecision.None;
			return false;
		}
	}

	private bool checkContinueReload(float timeSinceChange, Enemy enemy)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Invalid comparison between Unknown and I4
		BotComponent bot = base.Bot;
		if (bot.Decision.CurrentSelfDecision != ESelfDecision.Reload)
		{
			return false;
		}
		BotWeaponManager weaponManager = bot.BotOwner.WeaponManager;
		if (weaponManager == null)
		{
			return false;
		}
		BotReload reload = weaponManager.Reload;
		if (reload == null)
		{
			return false;
		}
		if (!reload.Reloading)
		{
			return false;
		}
		Weapon currentWeapon = weaponManager.CurrentWeapon;
		if (currentWeapon == null)
		{
			return false;
		}
		if ((int)currentWeapon.ReloadMode == 1 && reload.BulletCount >= 3 && enemy != null && enemy.IsVisible && Time.time - enemy.Vision.VisibleStartTime > 0.25f)
		{
			reload.TryStopReload();
		}
		reload.CheckReloadLongTime();
		if (timeSinceChange > 8f)
		{
			base.Bot.SelfActions.BotCancelReload();
			return false;
		}
		return true;
	}

	private bool checkContinueSurgery(out ESelfDecision Decision)
	{
		BotOwner botOwner = base.BotOwner;
		if (((botOwner != null) ? botOwner.Medecine : null) == null)
		{
			Decision = ESelfDecision.None;
			return false;
		}
		if (base.Bot.Medical.Surgery.AreaClearForSurgery && !checkDecisionTooLong())
		{
			Decision = ESelfDecision.Surgery;
			return true;
		}
		base.Bot.Medical.TryCancelHeal();
		Decision = ESelfDecision.None;
		return false;
	}

	private bool checkContinueFirstAid(float timeSinceChange, out ESelfDecision Decision, Enemy enemy)
	{
		BotOwner botOwner = base.BotOwner;
		if (((botOwner != null) ? botOwner.Medecine : null) == null)
		{
			Decision = ESelfDecision.None;
			return false;
		}
		if (timeSinceChange > 6f)
		{
			base.Bot.Medical.TryCancelHeal();
			Decision = ESelfDecision.None;
			return false;
		}
		Decision = ESelfDecision.FirstAid;
		return true;
	}

	private bool checkContinueStims(float timeSinceChange, out ESelfDecision Decision)
	{
		BotOwner botOwner = base.BotOwner;
		if (((botOwner != null) ? botOwner.Medecine : null) == null)
		{
			Decision = ESelfDecision.None;
			return false;
		}
		if (timeSinceChange > 3f)
		{
			base.Bot.Medical.TryCancelHeal();
			Decision = ESelfDecision.None;
			return false;
		}
		Decision = ESelfDecision.Stims;
		return true;
	}

	private bool checkDecisionTooLong()
	{
		return Time.time - base.Bot.Decision.ChangeDecisionTime > 60f;
	}

	private bool startUseStims()
	{
		if (!CanUseStims)
		{
			return false;
		}
		if (!base.Bot.Memory.Health.Dying && !base.Bot.Memory.Health.BadlyInjured)
		{
			return false;
		}
		if (base.Bot.EnemyController.AtPeace)
		{
			return true;
		}
		if (base.Bot.Decision.RunningToCover)
		{
			return true;
		}
		foreach (Enemy knownEnemy in base.Bot.EnemyController.EnemyLists.KnownEnemies)
		{
			if (!ShallUseStimsCheckEnemy(knownEnemy))
			{
				return false;
			}
		}
		return true;
	}

	private bool startFirstAid()
	{
		if (base.Bot.Medical.TimeSinceShot < 0.25f)
		{
			return false;
		}
		if (!CanUseFirstAid)
		{
			return false;
		}
		if (base.Bot.Memory.Health.Healthy)
		{
			return false;
		}
		if (base.Bot.Decision.RunningToCover)
		{
			return true;
		}
		foreach (Enemy knownEnemy in base.Bot.EnemyController.EnemyLists.KnownEnemies)
		{
			if (!ShallFirstAidCheckEnemy(knownEnemy))
			{
				return false;
			}
		}
		return true;
	}

	private static bool ShallUseStimsCheckEnemy(Enemy enemy)
	{
		if (enemy == null)
		{
			return true;
		}
		if (!enemy.Seen && !enemy.Heard)
		{
			return true;
		}
		if (enemy.InLineOfSight)
		{
			return false;
		}
		float timeSinceLastKnownUpdated = enemy.TimeSinceLastKnownUpdated;
		if (!enemy.Seen && timeSinceLastKnownUpdated > 3f)
		{
			return true;
		}
		EPathDistance ePathDistance = enemy.EPathDistance;
		if (1 == 0)
		{
		}
		bool result = ePathDistance switch
		{
			EPathDistance.VeryClose => timeSinceLastKnownUpdated > 6f, 
			EPathDistance.Close => timeSinceLastKnownUpdated > 3f, 
			EPathDistance.Mid => enemy.TimeSinceSeen > 2f, 
			EPathDistance.Far => true, 
			EPathDistance.VeryFar => true, 
			_ => false, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private bool ShallFirstAidCheckEnemy(Enemy enemy)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Invalid comparison between Unknown and I4
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Invalid comparison between Unknown and I4
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Invalid comparison between Unknown and I4
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Invalid comparison between Unknown and I4
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Invalid comparison between Unknown and I4
		if (enemy == null || !enemy.CheckValid())
		{
			return true;
		}
		if (!enemy.Seen && !enemy.Heard)
		{
			return true;
		}
		if (enemy.InLineOfSight)
		{
			return false;
		}
		float timeSinceLastKnownUpdated = enemy.TimeSinceLastKnownUpdated;
		ETagStatus healthStatus = base.Bot.Memory.Health.HealthStatus;
		if ((int)healthStatus != 4096 && (int)healthStatus != 8192 && !enemy.Seen && timeSinceLastKnownUpdated > 8f)
		{
			return true;
		}
		if (1 == 0)
		{
		}
		bool result;
		if ((int)healthStatus != 2048)
		{
			if ((int)healthStatus != 4096)
			{
				if ((int)healthStatus == 8192)
				{
					EPathDistance ePathDistance = enemy.EPathDistance;
					if (1 == 0)
					{
					}
					bool flag = ePathDistance switch
					{
						EPathDistance.VeryClose => timeSinceLastKnownUpdated > 15f && (!enemy.Seen || enemy.TimeSinceSeen > 15f), 
						EPathDistance.Close => timeSinceLastKnownUpdated > 10f && (!enemy.Seen || enemy.TimeSinceSeen > 10f), 
						EPathDistance.Mid => timeSinceLastKnownUpdated > 4f && (!enemy.Seen || enemy.TimeSinceSeen > 4f), 
						EPathDistance.Far => timeSinceLastKnownUpdated > 3f && (!enemy.Seen || enemy.TimeSinceSeen > 3f), 
						EPathDistance.VeryFar => timeSinceLastKnownUpdated > 2f && (!enemy.Seen || enemy.TimeSinceSeen > 2f), 
						_ => false, 
					};
					if (1 == 0)
					{
					}
					result = flag;
				}
				else
				{
					result = false;
				}
			}
			else
			{
				EPathDistance ePathDistance2 = enemy.EPathDistance;
				if (1 == 0)
				{
				}
				bool flag = ePathDistance2 switch
				{
					EPathDistance.VeryClose => timeSinceLastKnownUpdated > 18f && (!enemy.Seen || enemy.TimeSinceSeen > 18f), 
					EPathDistance.Close => timeSinceLastKnownUpdated > 12f && (!enemy.Seen || enemy.TimeSinceSeen > 12f), 
					EPathDistance.Mid => timeSinceLastKnownUpdated > 6f && (!enemy.Seen || enemy.TimeSinceSeen > 6f), 
					EPathDistance.Far => timeSinceLastKnownUpdated > 4f && (!enemy.Seen || enemy.TimeSinceSeen > 4f), 
					EPathDistance.VeryFar => timeSinceLastKnownUpdated > 2f && (!enemy.Seen || enemy.TimeSinceSeen > 2f), 
					_ => false, 
				};
				if (1 == 0)
				{
				}
				result = flag;
			}
		}
		else
		{
			EPathDistance ePathDistance3 = enemy.EPathDistance;
			if (1 == 0)
			{
			}
			bool flag = ePathDistance3 switch
			{
				EPathDistance.VeryClose => timeSinceLastKnownUpdated > 20f && (!enemy.Seen || enemy.TimeSinceSeen > 20f), 
				EPathDistance.Close => timeSinceLastKnownUpdated > 15f && (!enemy.Seen || enemy.TimeSinceSeen > 15f), 
				EPathDistance.Mid => enemy.TimeSinceSeen > 8f && (!enemy.Seen || enemy.TimeSinceSeen > 8f), 
				EPathDistance.Far => enemy.TimeSinceSeen > 5f && (!enemy.Seen || enemy.TimeSinceSeen > 5f), 
				EPathDistance.VeryFar => enemy.TimeSinceSeen > 3f && (!enemy.Seen || enemy.TimeSinceSeen > 3f), 
				_ => false, 
			};
			if (1 == 0)
			{
			}
			result = flag;
		}
		if (1 == 0)
		{
		}
		return result;
	}

	private static bool CheckReloadByAmmoRemaining(Enemy enemy, float ammoRatio)
	{
		if (enemy.IsVisible && enemy.CanShoot)
		{
			return false;
		}
		float timeSinceLastKnownUpdated = enemy.TimeSinceLastKnownUpdated;
		if (timeSinceLastKnownUpdated < 0.2f)
		{
			return false;
		}
		EPathDistance ePathDistance = enemy.EPathDistance;
		if (ammoRatio > 0.66f)
		{
			if (timeSinceLastKnownUpdated > 15f)
			{
				return true;
			}
			switch (ePathDistance)
			{
			case EPathDistance.VeryClose:
				if (timeSinceLastKnownUpdated > 90f)
				{
					return true;
				}
				break;
			case EPathDistance.Close:
				if (timeSinceLastKnownUpdated > 12f)
				{
					return true;
				}
				break;
			case EPathDistance.Mid:
				if (timeSinceLastKnownUpdated > 6f)
				{
					return true;
				}
				break;
			case EPathDistance.Far:
				if (timeSinceLastKnownUpdated > 3f)
				{
					return true;
				}
				break;
			case EPathDistance.VeryFar:
				if (timeSinceLastKnownUpdated > 3f)
				{
					return true;
				}
				break;
			}
			return false;
		}
		if (ammoRatio > 0.4f)
		{
			if (timeSinceLastKnownUpdated > 20f)
			{
				return true;
			}
			if (1 == 0)
			{
			}
			bool result = ePathDistance switch
			{
				EPathDistance.VeryClose => timeSinceLastKnownUpdated > 60f, 
				EPathDistance.Close => timeSinceLastKnownUpdated > 8f, 
				EPathDistance.Mid => timeSinceLastKnownUpdated > 4f, 
				EPathDistance.Far => timeSinceLastKnownUpdated > 2f, 
				EPathDistance.VeryFar => timeSinceLastKnownUpdated > 2f, 
				_ => false, 
			};
			if (1 == 0)
			{
			}
			return result;
		}
		if (ammoRatio > 0.2f)
		{
			if (enemy.TimeSinceSeen > 4f)
			{
				return true;
			}
			switch (ePathDistance)
			{
			case EPathDistance.VeryClose:
				if (timeSinceLastKnownUpdated > 2f)
				{
					return true;
				}
				break;
			case EPathDistance.Close:
				if (timeSinceLastKnownUpdated > 2f)
				{
					return true;
				}
				break;
			case EPathDistance.Mid:
				if (timeSinceLastKnownUpdated > 1f)
				{
					return true;
				}
				break;
			case EPathDistance.Far:
				if (timeSinceLastKnownUpdated > 1f)
				{
					return true;
				}
				break;
			case EPathDistance.VeryFar:
				if (timeSinceLastKnownUpdated > 1f)
				{
					return true;
				}
				break;
			}
			return false;
		}
		if (timeSinceLastKnownUpdated > 2f)
		{
			return true;
		}
		return false;
	}

	public bool LowOnAmmo(float ratio = 0.3f)
	{
		return AmmoRatio < ratio;
	}

	private float getAmmoRatio(BotReload reload)
	{
		float result = _ammoRatio;
		try
		{
			if (reload != null)
			{
				int bulletCount = reload.BulletCount;
				int maxBulletCount = reload.MaxBulletCount;
				result = (float)bulletCount / (float)maxBulletCount;
			}
		}
		catch
		{
		}
		return result;
	}
}
