using SAIN.Components;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class AimHitEffectClass : BotBase
{
	private Vector3 _affectVector = Vector3.zero;

	private float _affectAmount;

	private float _finishDelay = 1f;

	private bool _affectActive;

	private float _timeFinished;

	private float EFFECT_MIN_ANGLE => _settings.DAMAGE_BASE_MIN_ANGLE;

	private float EFFECT_MAX_ANGLE => _settings.DAMAGE_BASE_MAX_ANGLE;

	private float DAMAGE_BASELINE => _settings.DAMAGE_RECEIVED_BASELINE;

	private float DAMAGE_MIN_MOD => _settings.DAMAGE_MIN_MOD;

	private float DAMAGE_MAX_MOD => _settings.DAMAGE_MAX_MOD;

	private float DAMAGE_MANUAL_MODIFIER => _settings.DAMAGE_MANUAL_MODIFIER;

	private bool DAMAGE_USE_HIT_OFFSET_DIR => _settings.USE_HIT_POINT_DIRECTION;

	private float DAMAGE_HIT_OFFSET_BASE_DIST => _settings.HIT_POINT_DIRECTION_BASE_DISTANCE;

	private HitEffectSettings _settings => GlobalSettingsClass.Instance.Aiming.HitEffects;

	public AimHitEffectClass(BotComponent bot)
		: base(bot)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	public Vector3 ApplyEffect(Vector3 dir)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (_affectActive)
		{
			decayAffect();
			Vector3 val = _affectVector * _affectAmount;
			return ((Vector3)(ref dir)).normalized + val;
		}
		return dir;
	}

	private float calcDamageMod(DamageInfoStruct DamageInfoStruct)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		float num = DamageInfoStruct.Damage / DAMAGE_BASELINE;
		num = Mathf.Clamp(num, DAMAGE_MIN_MOD, DAMAGE_MAX_MOD) * DAMAGE_MANUAL_MODIFIER;
		if (_affectActive)
		{
			num *= 0.5f;
		}
		return num;
	}

	private Vector3 getHitReactionDir(DamageInfoStruct DamageInfoStruct)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Vector3 hitPoint = DamageInfoStruct.HitPoint;
		Vector3 bodyPosition = base.Bot.Transform.BodyPosition;
		Vector3 val = hitPoint - bodyPosition;
		Vector3 result = ((Vector3)(ref val)).normalized * DAMAGE_HIT_OFFSET_BASE_DIST;
		result.y *= 0.5f;
		return result;
	}

	public void GetHit(DamageInfoStruct DamageInfoStruct)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float num = calcDamageMod(DamageInfoStruct);
		if (DAMAGE_USE_HIT_OFFSET_DIR)
		{
			Vector3 val = getHitReactionDir(DamageInfoStruct) * num;
			_affectVector += val;
		}
		else
		{
			float num2 = Mathf.Clamp(EFFECT_MIN_ANGLE * num, 0f, 90f);
			float num3 = Mathf.Clamp(EFFECT_MAX_ANGLE * num, 0f, 90f);
			float degX = Random.Range(0f - num2, 0f - num3) * 0.5f;
			float degY = (float)EFTMath.RandomSing() * Random.Range(num2, num3);
			Vector3 lookDirection = base.Bot.Transform.LookDirection;
			_affectVector = Vector.Rotate(_affectVector + lookDirection, degX, degY, 0f) - lookDirection;
		}
		BotGlobalAimingSettings aiming = base.BotOwner.Settings.FileSettings.Aiming;
		_affectActive = true;
		_finishDelay = aiming.BASE_HIT_AFFECTION_DELAY_SEC * Mathf.Clamp(num, 0f, 1.5f) * Random.Range(0.8f, 1.2f);
		_timeFinished = Time.time + _finishDelay;
	}

	public void decayAffect()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (_affectActive)
		{
			float num = _timeFinished - Time.time;
			if (num <= 0f)
			{
				_affectActive = false;
				_affectVector = Vector3.zero;
			}
			else
			{
				_affectAmount = num / _finishDelay;
			}
		}
	}
}
