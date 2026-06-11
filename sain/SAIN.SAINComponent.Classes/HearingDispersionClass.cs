using System;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class HearingDispersionClass : BotSubClass<SAINHearingSensorClass>, IBotClass, IDisposable
{
	public HearingDispersionClass(SAINHearingSensorClass hearing)
		: base(hearing)
	{
	}

	public Vector3 CalcRandomizedPosition(AISoundData Sound, float addDispersion)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		float baseDispersion = getBaseDispersion(Sound.PlayerDistance, Sound.SoundType);
		float num = getDispersionModifier(Sound.Enemy) * addDispersion;
		float num2 = baseDispersion * num;
		HearingSettings hearing = GlobalSettingsClass.Instance.Hearing;
		num2 = Mathf.Clamp(num2, 0f, hearing.HEAR_DISPERSION_MAX_DISPERSION);
		float min = ((Sound.PlayerDistance < hearing.HEAR_DISPERSION_MIN_DISTANCE_THRESH) ? 0f : hearing.HEAR_DISPERSION_MIN);
		Vector3 randomizedDirection = getRandomizedDirection(num2, min);
		if (SAINPlugin.DebugSettings.Logs.DebugHearing)
		{
			Logger.LogDebug($"Dispersion: [{((Vector3)(ref randomizedDirection)).magnitude}] Distance: [{Sound.PlayerDistance}] Base Dispersion: [{baseDispersion}] DispersionModifier [{num}] Final Dispersion: [{num2}] : SoundType: [{Sound.SoundType}]");
		}
		Vector3 val = Sound.Position + randomizedDirection;
		Vector3 val2 = val - base.Bot.Position;
		return base.Bot.Position + ((Vector3)(ref val2)).normalized * Sound.PlayerDistance;
	}

	private float getBaseDispersion(float enemyDistance, SAINSoundType soundType)
	{
		HearingSettings hearing = GlobalSettingsClass.Instance.Hearing;
		if (!hearing.HEAR_DISPERSION_VALUES.TryGetValue(soundType, out var value))
		{
			value = 12.5f;
		}
		return enemyDistance / value;
	}

	private float getDispersionModifier(Enemy Enemy)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Vector3 lookDirection = base.Bot.LookDirection;
		float num = Vector3.Dot(((Vector3)(ref lookDirection)).normalized, Enemy.EnemyDirectionNormal);
		float num2 = (num + 1f) / 2f;
		HearingSettings hearing = GlobalSettingsClass.Instance.Hearing;
		return Mathf.Lerp(hearing.HEAR_DISPERSION_ANGLE_MULTI_MAX, hearing.HEAR_DISPERSION_ANGLE_MULTI_MIN, num2);
	}

	private Vector3 getRandomizedDirection(float dispersion, float min = 0.5f)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		float num = Random.Range(0f - dispersion, dispersion);
		float num2 = Random.Range(0f - dispersion, dispersion);
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(num, 0f, num2);
		if (min > 0f && ((Vector3)(ref val)).sqrMagnitude < min * min)
		{
			val = Vector3.Normalize(val) * min;
			return val;
		}
		return val;
	}

	public Vector3 GetEstimatedPoint(Vector3 source, float distance)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 onUnitSphere = Random.onUnitSphere;
		onUnitSphere.y = 0f;
		onUnitSphere *= distance / 10f;
		return source + onUnitSphere;
	}
}
