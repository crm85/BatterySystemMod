using System;
using EFT;
using EFT.HealthSystem;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Preset.BotSettings.SAINSettings.Categories;
using SAIN.Preset.GlobalSettings;
using SAIN.Preset.GlobalSettings.Categories;
using SAIN.Preset.Personalities;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class SAINBotSuppressClass : BotComponentClassBase
{
	private Enemy EnemyBeingSuppressed;

	public bool SuppressingTarget;

	private float _suppressTime;

	private float _decayTime;

	private float _tickTime;

	private TemporaryStatModifiers _temporaryStatModifiers;

	public Enemy LastSuppressByEnemy { get; private set; }

	public ESuppressionState CurrentState { get; private set; }

	public ESuppressionState LastState { get; private set; }

	public float SuppressionNumber { get; private set; }

	public bool IsSuppressed => CurrentState >= ESuppressionState.Medium;

	public bool IsHeavySuppressed => CurrentState >= ESuppressionState.Heavy;

	private MindSettings _settings => BotBase.GlobalSettings.Mind;

	public event Action<ESuppressionState> OnSuppressionStateChanged;

	public SAINBotSuppressClass(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
	}

	public override void Init()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved += clearLastSuppEnemy;
		base.Init();
	}

	public override void ManualUpdate()
	{
		checkState();
		decaySuppression();
		if (SuppressingTarget)
		{
			Enemy enemyBeingSuppressed = EnemyBeingSuppressed;
			if (enemyBeingSuppressed != null)
			{
				Player enemyPlayer = enemyBeingSuppressed.EnemyPlayer;
				bool? obj;
				if (enemyPlayer == null)
				{
					obj = null;
				}
				else
				{
					IHealthController healthController = enemyPlayer.HealthController;
					obj = ((healthController != null) ? new bool?(healthController.IsAlive) : ((bool?)null));
				}
				bool? flag = obj;
				if (flag == true)
				{
					if (!base.Bot.HasEnemy || base.Bot.Enemy != EnemyBeingSuppressed)
					{
						ResetSuppressing();
					}
					else if (_suppressTime < Time.time)
					{
						ResetSuppressing();
					}
					goto IL_00c8;
				}
			}
			ResetSuppressing();
		}
		goto IL_00c8;
		IL_00c8:
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved -= clearLastSuppEnemy;
		base.Dispose();
	}

	public bool TrySuppressAnyEnemy(Enemy priorityEnemy, EnemyList knownEnemies, float minimumAmmoRatio = 0.33f, int minimumBullets = 2)
	{
		if (SuppressingTarget && _suppressTime > Time.time)
		{
			return true;
		}
		BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
		BotReload val = ((weaponManager != null) ? weaponManager.Reload : null);
		int num = 0;
		float num2 = 0f;
		if (val != null)
		{
			num = val.BulletCount;
			num2 = (float)num / (float)val.MaxBulletCount;
		}
		if (num2 >= minimumAmmoRatio && num >= minimumBullets)
		{
			if (TrySuppressEnemy(priorityEnemy))
			{
				return true;
			}
			knownEnemies.SortBy(EnemyList.EBotListSortType.VisiblePathPointDistanceToEnemy);
			foreach (Enemy knownEnemy in knownEnemies)
			{
				if (TrySuppressEnemy(knownEnemy))
				{
					return true;
				}
			}
		}
		ResetSuppressing();
		return false;
	}

	public bool TrySuppressEnemy(Enemy Enemy)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (Enemy != null && !Enemy.IsZombie && !Enemy.IsVisible && ((Enemy.Seen && Enemy.TimeSinceSeen < 4f) || Enemy.Status.ShotAtMeRecently || Enemy.Status.ShotByEnemyRecently))
		{
			Vector3? suppressionTarget = Enemy.SuppressionTarget;
			if (suppressionTarget.HasValue)
			{
				return base.Bot.Suppression.SuppressPosition(suppressionTarget.Value, Enemy);
			}
		}
		return false;
	}

	public bool SuppressPosition(Vector3 position, Enemy Enemy)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (Enemy == null || !base.Bot.ManualShoot.TryShoot(Enemy, position, checkFF: true, EShootReason.Suppress))
		{
			ResetSuppressing();
			return false;
		}
		SuppressingTarget = true;
		Enemy.Status.EnemyIsSuppressed = true;
		EnemyBeingSuppressed = Enemy;
		if (_suppressTime < Time.time)
		{
			float num = ((base.Bot.Info.WeaponInfo.EWeaponClass == EWeaponClass.machinegun) ? 0.05f : 0.25f);
			_suppressTime = Time.time + num * Random.Range(0.66f, 1.33f);
		}
		return true;
	}

	public void ResetSuppressing()
	{
		if (EnemyBeingSuppressed != null || SuppressingTarget)
		{
			if (EnemyBeingSuppressed != null)
			{
				EnemyBeingSuppressed.Status.EnemyIsSuppressed = false;
				EnemyBeingSuppressed = null;
			}
			SuppressingTarget = false;
			base.Bot.ManualShoot.Reset();
			_suppressTime = 0f;
		}
	}

	public void CheckAddSuppression(Enemy enemy, float distance, float amount = -1f)
	{
		if (!_settings.SUPP_TOGGLE)
		{
			return;
		}
		float resistance = getResistance();
		if (!(resistance >= 1f))
		{
			if (amount <= 0f)
			{
				amount = getSuppNum(enemy);
			}
			float num = scaleSuppDist(amount * _settings.SUPP_AMOUNT_MULTI, distance);
			if (!(num <= 0f))
			{
				LastSuppressByEnemy = enemy;
				float addAmount = calcResistance(num, resistance);
				clampAndUpdateSuppression(addAmount);
			}
		}
	}

	private static float scaleSuppDist(float suppNum, float distance)
	{
		MindSettings mind = GlobalSettingsClass.Instance.Mind;
		if (distance < mind.SUPP_DISTANCE_AMP_DIST)
		{
			return suppNum * mind.SUPP_DISTANCE_AMP_AMOUNT;
		}
		float sUPP_DISTANCE_SCALE_END = mind.SUPP_DISTANCE_SCALE_END;
		float sUPP_DISTANCE_SCALE_START = mind.SUPP_DISTANCE_SCALE_START;
		float num = (distance - sUPP_DISTANCE_SCALE_START) / (sUPP_DISTANCE_SCALE_END - sUPP_DISTANCE_SCALE_START);
		return Mathf.Lerp(suppNum, 0f, num);
	}

	private float getResistance()
	{
		SAINMindSettings mind = base.Bot.Info.FileSettings.Mind;
		if (mind == null)
		{
			return 1f;
		}
		PersonalityGeneralSettings general = base.Bot.Info.PersonalitySettings.General;
		if (general == null)
		{
			return Mathf.Clamp01(mind.SuppressionResistance);
		}
		return Mathf.Lerp(Mathf.Clamp01(mind.SuppressionResistance), Mathf.Clamp01(general.SuppressionResistance), 0.5f).Round100();
	}

	private float getSuppNum(Enemy enemy)
	{
		WeaponInfo currentWeaponInfo = enemy.EnemyPlayerComponent.Equipment.CurrentWeaponInfo;
		if (currentWeaponInfo == null)
		{
			if (SAINPlugin.DebugMode)
			{
				Logger.LogWarning("Could not find Weapon to check suppression amount!");
			}
			return 2f;
		}
		if (BotBase.GlobalSettings.Mind.SUPP_AMOUNTS.TryGetValue(currentWeaponInfo.AmmoCaliber, out var value))
		{
			return value;
		}
		if (SAINPlugin.DebugMode)
		{
			Logger.LogWarning($"Could not find [{currentWeaponInfo.AmmoCaliber}] to check suppression amount!");
		}
		if (BotBase.GlobalSettings.Mind.SUPP_AMOUNTS.TryGetValue(ECaliber.Default, out value))
		{
			return value;
		}
		if (SAINPlugin.DebugMode)
		{
			Logger.LogWarning("Could not find Default Caliber Value to check suppression amount!");
		}
		return 2f;
	}

	private void checkState()
	{
		if (!(_tickTime < Time.time))
		{
			return;
		}
		_tickTime = Time.time + _settings.SUP_CHECK_FREQ;
		if (SuppressionNumber <= 0f)
		{
			if (CurrentState != ESuppressionState.None)
			{
				applyNewState(ESuppressionState.None, null);
			}
			return;
		}
		SuppressionConfig suppressionConfig;
		ESuppressionState eSuppressionState = SuppressionHelpers.FindActiveState(SuppressionNumber, out suppressionConfig);
		if (CurrentState != eSuppressionState)
		{
			applyNewState(eSuppressionState, suppressionConfig);
		}
	}

	private void decaySuppression()
	{
		if (SuppressionNumber > 0f && _decayTime < Time.time)
		{
			_decayTime = Time.time + _settings.SUP_DECAY_FREQ;
			clampAndUpdateSuppression(0f - _settings.SUP_DECAY_AMOUNT);
		}
	}

	private void clampAndUpdateSuppression(float addAmount)
	{
		SuppressionNumber = Mathf.Clamp(SuppressionNumber + addAmount, 0f, _settings.SUPP_MAX_NUM);
	}

	private void applyNewState(ESuppressionState newState, SuppressionConfig config)
	{
		LastState = CurrentState;
		CurrentState = newState;
		clearModifiers();
		if (newState == ESuppressionState.None || config == null)
		{
			return;
		}
		_temporaryStatModifiers = createMods(config);
		BotOwner botOwner = base.BotOwner;
		if (botOwner != null)
		{
			BotDifficultySettingsClass settings = botOwner.Settings;
			if (settings != null)
			{
				GClass596 current = settings.Current;
				if (current != null)
				{
					current.Apply(_temporaryStatModifiers.Modifiers, -1f);
				}
			}
		}
		this.OnSuppressionStateChanged?.Invoke(newState);
	}

	private void clearModifiers()
	{
		if (_temporaryStatModifiers != null)
		{
			if (_temporaryStatModifiers.Modifiers.IsApplyed)
			{
				base.BotOwner.Settings.Current.Dismiss(_temporaryStatModifiers.Modifiers);
			}
			_temporaryStatModifiers = null;
		}
	}

	private static TemporaryStatModifiers createMods(SuppressionConfig config)
	{
		float num = Mathf.Clamp(GlobalSettingsClass.Instance.Mind.SUPP_STRENGTH_MULTI, 0.01f, 100f);
		return new TemporaryStatModifiers((config.PrecisionSpeedCoef * num).Round100(), (config.AccuracySpeedCoef * num).Round100(), (config.GainSightCoef * num).Round100(), (config.ScatteringCoef * num).Round100(), (config.ScatteringCoef * num).Round100(), config.VisibleDistCoef, config.HearingDistCoef);
	}

	private static float calcResistance(float value, float resistance)
	{
		if (value == 1f)
		{
			return 1f;
		}
		resistance = Mathf.Clamp01(resistance);
		return Mathf.Lerp(value, 0f, resistance);
	}

	private void clearLastSuppEnemy(string profileId, Enemy enemy)
	{
		if (LastSuppressByEnemy != null && LastSuppressByEnemy.IsSame(enemy))
		{
			LastSuppressByEnemy = null;
		}
		if (EnemyBeingSuppressed != null && EnemyBeingSuppressed == enemy)
		{
			ResetSuppressing();
		}
	}
}
