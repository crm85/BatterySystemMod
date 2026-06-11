using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SAIN.Components;
using SAIN.Models.Enums;
using SAIN.Preset;
using SAIN.Preset.BotSettings.SAINSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class GrenadeThrowDecider : BotSubClass<BotGrenadeManager>, IBotDecisionClass
{
	private bool _grenadesEnabled = true;

	private bool _canThrowGrenades = true;

	private bool _canThrowAtVisEnemies = false;

	private bool _canThrowWhileSprint = false;

	private float _timeSinceSeenBeforeThrow = 3f;

	private float _maxTimeSinceUpdatedCanThrow = 120f;

	private float _minEnemyDistToThrow = 8f;

	private float _throwGrenadeFreq = 5f;

	private float _throwGrenadeFreqMax = 10f;

	private float _minFriendlyDistToThrow = 8f;

	private float _minFriendlyDistToThrow_SQR = 64f;

	private float _blindCornerDistToThrow = 5f;

	private float _blindCornerDistToLastKnown_Max_SQR = 25f;

	private float _checkThrowPos_HeightOffset = 0.25f;

	private float _maxEnemyDistToCheckThrow = 75f;

	private float _friendlyCloseRecheckTime = 3f;

	private float _sayNeedGrenadeFreq = 10f;

	private float _sayNeedGrenadeChance = 5f;

	private const float THROW_FREQUENCY_RANDOMIZATION_FACTOR = 2f;

	private const float MIN_THROW_DISPERSION = 0.5f;

	private const float MAX_THROW_DISPERSION = 5f;

	private const float MIN_THROW_DISTANCE_DISPERSION = 10f;

	private const float MAX_THROW_DISTANCE_DISPERSION = 50f;

	private float _nextSayNeedGrenadeTime;

	private float _minThrowDistPercent;

	private float _nextPosibleAttempt;

	private static AIGreandeAng[] _indoorAngles;

	private static AIGreandeAng[] _outdoorAngles;

	private float _maxPower => base.BotOwner.WeaponManager.Grenades.MaxPower;

	public GrenadeThrowDecider(BotGrenadeManager ThrowWeapItemClass)
		: base(ThrowWeapItemClass)
	{
		base.CanEverTick = false;
	}

	protected override void UpdatePresetSettings(SAINPresetClass preset)
	{
		_grenadesEnabled = preset.GlobalSettings.General.BotsUseGrenades;
		SAINSettingsClass fileSettings = base.Bot.Info.FileSettings;
		_canThrowGrenades = fileSettings.Core.CanGrenade;
		_canThrowAtVisEnemies = fileSettings.Grenade.ThrowAtVisibleEnemies;
		_canThrowWhileSprint = fileSettings.Grenade.CanThrowWhileSprinting;
		_minEnemyDistToThrow = fileSettings.Grenade.MinEnemyDistance;
		_minFriendlyDistToThrow = fileSettings.Grenade.MinFriendlyDistance;
		_minFriendlyDistToThrow_SQR = _minFriendlyDistToThrow * _minFriendlyDistToThrow;
		_throwGrenadeFreq = fileSettings.Grenade.ThrowGrenadeFrequency;
		_throwGrenadeFreqMax = fileSettings.Grenade.ThrowGrenadeFrequency_MAX;
		_minThrowDistPercent = 0.66f;
		_blindCornerDistToThrow = 10f;
		_blindCornerDistToLastKnown_Max_SQR = _blindCornerDistToThrow * _blindCornerDistToThrow;
		_checkThrowPos_HeightOffset = 0.25f;
	}

	public bool GetDecision(Enemy enemy, out string reason)
	{
		if (enemy.IsAI && !BotBase.GlobalSettings.General.BotVsBotGrenade)
		{
			reason = "noGoodTarget";
			return false;
		}
		BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
		if (weaponManager != null && weaponManager.Grenades.ThrowindNow)
		{
			reason = "throwingNow";
			return true;
		}
		if (!checkCanThrow(out reason))
		{
			return false;
		}
		if (!canThrowAtEnemy(enemy, out reason))
		{
			return false;
		}
		BotGrenadeController grenades = base.BotOwner.WeaponManager.Grenades;
		if (!grenades.HaveGrenade)
		{
			_nextPosibleAttempt = Time.time + Random.Range(_throwGrenadeFreq, _throwGrenadeFreqMax);
			sayNeedNades();
			reason = "noNades";
			return false;
		}
		if (findThrowTarget(enemy) && tryThrowGrenade())
		{
			_nextPosibleAttempt = Time.time + Random.Range(_throwGrenadeFreq, _throwGrenadeFreqMax);
			reason = "startThrow";
			return true;
		}
		reason = "noGoodTarget";
		return false;
	}

	private bool checkCanThrow(out string reason)
	{
		if (!_grenadesEnabled)
		{
			reason = "grenadesDisabledGlobal";
			return false;
		}
		if (!_canThrowGrenades)
		{
			reason = "grenadesDisabled";
			return false;
		}
		BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
		if (weaponManager != null)
		{
			if (weaponManager.Selector.IsChanging)
			{
				reason = "changingWeapon";
				return false;
			}
			if (weaponManager.Reload.Reloading)
			{
				reason = "reloading";
				return false;
			}
		}
		if (_nextPosibleAttempt > Time.time)
		{
			reason = "nextAttemptTime";
			return false;
		}
		if (!_canThrowWhileSprint && (base.Player.IsSprintEnabled || base.Bot.Mover.PathFollower.Running))
		{
			reason = "running";
			return false;
		}
		if (base.Player.HandsController.IsInInteractionStrictCheck())
		{
			reason = "handsController Busy";
			return false;
		}
		reason = "canThrow";
		return true;
	}

	private bool canThrowAtEnemy(Enemy enemy, out string reason)
	{
		if (!_canThrowAtVisEnemies)
		{
			if (enemy.IsVisible || enemy.InLineOfSight)
			{
				reason = "enemyVisible";
				return false;
			}
			if (enemy.TimeSinceSeen < _timeSinceSeenBeforeThrow)
			{
				reason = "enemySeenRecent";
				return false;
			}
		}
		if (enemy.TimeSinceLastKnownUpdated > _maxTimeSinceUpdatedCanThrow)
		{
			reason = "lastUpdatedTooLong";
			return false;
		}
		EnemyPlace lastKnownPlace = enemy.KnownPlaces.LastKnownPlace;
		if (lastKnownPlace == null)
		{
			reason = "nullLastKnown";
			return false;
		}
		if (lastKnownPlace.DistanceToBot > _maxEnemyDistToCheckThrow)
		{
			reason = "tooFar";
			return false;
		}
		if (lastKnownPlace.DistanceToBot < _minEnemyDistToThrow)
		{
			reason = "tooClose";
			return false;
		}
		reason = string.Empty;
		return true;
	}

	private bool findThrowTarget(Enemy enemy)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		EnemyPlace lastKnownPlace = enemy.KnownPlaces.LastKnownPlace;
		if (lastKnownPlace != null)
		{
			Vector3 position = lastKnownPlace.Position;
			if (!checkFriendlyDistances(position))
			{
				return false;
			}
			AIGreandeAng[] possibleAngles = (base.Bot.Memory.Location.IsIndoors ? _indoorAngles : _outdoorAngles);
			if (tryThrowToPos(position, "LastKnownPosition", lastKnownPlace.DistanceToBot, possibleAngles))
			{
				return true;
			}
			if (checkCanThrowBlindCorner(enemy, position))
			{
				return true;
			}
		}
		return false;
	}

	private bool checkCanThrowBlindCorner(Enemy enemy, Vector3 lastKnownPos)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Vector3? val = enemy.Path.EnemyCorners.GroundPosition(ECornerType.Blind);
		if (!val.HasValue)
		{
			return false;
		}
		Vector3 value = val.Value;
		Vector3 val2 = value - lastKnownPos;
		float sqrMagnitude = ((Vector3)(ref val2)).sqrMagnitude;
		if (sqrMagnitude > _blindCornerDistToLastKnown_Max_SQR)
		{
			return false;
		}
		if (!checkFriendlyDistances(value))
		{
			return false;
		}
		if (tryThrowToPos(value, "BlindCornerToEnemy", Mathf.Sqrt(sqrMagnitude), default(AIGreandeAng)))
		{
			return true;
		}
		return false;
	}

	private bool tryThrowToPos(Vector3 pos, string posString, float distance, params AIGreandeAng[] possibleAngles)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		pos += Vector3.up * _checkThrowPos_HeightOffset;
		Vector3 weaponRoot = base.Bot.Transform.WeaponRoot;
		Vector3 val = pos - base.Bot.Position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		float throwDispersion = getThrowDispersion(pos, normalized, distance);
		if (canThrowAGrenade(weaponRoot, randomize(pos, normalized, throwDispersion), possibleAngles))
		{
			return true;
		}
		if (canThrowAGrenade(weaponRoot, randomize(pos, normalized, throwDispersion), possibleAngles))
		{
			return true;
		}
		if (canThrowAGrenade(weaponRoot, randomize(pos, normalized, throwDispersion) + Vector3.up * 0.5f, possibleAngles))
		{
			return true;
		}
		return false;
	}

	private float getThrowDispersion(Vector3 target, Vector3 targetDirectionNormal, float range)
	{
		float num = 0.5f;
		if (range <= 10f)
		{
			return num;
		}
		float num2 = 5f;
		if (range >= 50f)
		{
			return num2;
		}
		range = Mathf.Clamp(range, 10f, 50f);
		float num3 = 40f;
		float num4 = range - 10f;
		float num5 = num4 / num3;
		return Mathf.Lerp(num, num2, num5);
	}

	private Vector3 randomize(Vector3 target, Vector3 targetDirectionNormal, float dispersion)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return target + targetDirectionNormal * Random.Range(0f - dispersion, dispersion);
	}

	private bool tryThrowGrenade()
	{
		BotGrenadeController grenades = base.BotOwner.WeaponManager.Grenades;
		if (!grenades.ReadyToThrow)
		{
			return false;
		}
		if (!grenades.AIGreanageThrowData.IsUpToDate())
		{
			return false;
		}
		if (grenades.DoThrow())
		{
			return true;
		}
		return false;
	}

	private bool canThrowAGrenade(Vector3 from, Vector3 trg, params AIGreandeAng[] possibleAngles)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (_nextPosibleAttempt > Time.time)
		{
			return false;
		}
		if (base.Player.IsSprintEnabled || base.Bot.Mover.PathFollower.Running)
		{
			return false;
		}
		if (!checkFriendlyDistances(trg))
		{
			_nextPosibleAttempt = Time.time + _friendlyCloseRecheckTime;
			return false;
		}
		AIGreandeAng val = GClass1835.PickRandom<AIGreandeAng>((IReadOnlyList<AIGreandeAng>)possibleAngles);
		AIGreanageThrowData val2 = GClass557.CanThrowGrenade2(from, trg, _maxPower * 0.9f, val, -1f, _minThrowDistPercent);
		if (val2.CanThrow)
		{
			if (Physics.Raycast(from, val2.Direction, 1.5f, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
			{
				Logger.LogDebug("blocked by object, cant throw");
				return false;
			}
			base.BotOwner.WeaponManager.Grenades.SetThrowData(val2);
			return true;
		}
		return false;
	}

	private AIGreandeAng getAngleToThrow()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		AIGreandeAng[] array = (base.Bot.Memory.Location.IsIndoors ? _indoorAngles : _outdoorAngles);
		return GClass1835.PickRandom<AIGreandeAng>((IReadOnlyList<AIGreandeAng>)array);
	}

	private bool checkFriendlyDistances(Vector3 trg)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, BotComponent> members = base.Bot.Squad.Members;
		if (members == null || members.Count <= 1)
		{
			return true;
		}
		foreach (BotComponent value in members.Values)
		{
			if ((Object)(object)value != (Object)null)
			{
				Vector3 val = value.Position - trg;
				if (((Vector3)(ref val)).sqrMagnitude < _minFriendlyDistToThrow_SQR)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void sayNeedNades()
	{
		if (_nextSayNeedGrenadeTime < Time.time)
		{
			_nextSayNeedGrenadeTime = Time.time + _sayNeedGrenadeFreq;
			base.Bot.Talk.GroupSay((EPhraseTrigger)85, null, withGroupDelay: true, _sayNeedGrenadeChance);
		}
	}

	static GrenadeThrowDecider()
	{
		_indoorAngles = (AIGreandeAng[])(object)new AIGreandeAng[2]
		{
			default(AIGreandeAng),
			(AIGreandeAng)1
		};
		AIGreandeAng[] array = new AIGreandeAng[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		_outdoorAngles = (AIGreandeAng[])(object)array;
	}
}
