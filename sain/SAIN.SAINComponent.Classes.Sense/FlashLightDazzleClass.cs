using EFT;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Sense;

public class FlashLightDazzleClass : BotBase
{
	private TemporaryStatModifiers Modifiers = new TemporaryStatModifiers();

	private static float MaxDazzleRange => SAINPlugin.LoadedPreset.GlobalSettings.General.Flashlight.MaxDazzleRange;

	private static float Effectiveness => SAINPlugin.LoadedPreset.GlobalSettings.General.Flashlight.DazzleEffectiveness;

	public FlashLightDazzleClass(BotComponent owner)
		: base(owner)
	{
	}

	public void CheckIfDazzleApplied(Enemy enemy)
	{
		if (enemy != null && enemy.CheckValid() && enemy.IsVisible)
		{
			if (Modifiers.Modifiers.IsApplyed)
			{
				return;
			}
			FlashLightClass flashLightClass = enemy?.EnemyPlayerComponent?.Flashlight;
			if (flashLightClass != null)
			{
				bool usingNow = base.BotOwner.NightVision.UsingNow;
				if (((flashLightClass.WhiteLight || (usingNow && flashLightClass.IRLight)) && EnemyWithFlashlight(enemy)) || ((flashLightClass.Laser || (usingNow && flashLightClass.IRLaser)) && EnemyWithLaser(enemy)))
				{
					return;
				}
			}
		}
		base.ManualUpdate();
	}

	private bool EnemyWithFlashlight(Enemy enemy)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		float realDistance = enemy.RealDistance;
		if (realDistance < 80f && FlashlightVisionCheck(enemy.EnemyIPlayer))
		{
			Vector3 position = base.BotOwner.MyHead.position;
			Vector3 position2 = enemy.EnemyPlayer.WeaponRoot.position;
			Vector3 val = position - position2;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			val = position - position2;
			if (!Physics.Raycast(position2, normalized, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
			{
				float gainSightModif = 0.66f;
				float dazzleModif = ((realDistance < MaxDazzleRange) ? GetDazzleModifier(enemy) : 1f);
				ApplyDazzle(dazzleModif, gainSightModif);
			}
			return true;
		}
		return false;
	}

	private bool EnemyWithLaser(Enemy enemy)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		float realDistance = enemy.RealDistance;
		if (realDistance < 100f && LaserVisionCheck(enemy.EnemyIPlayer))
		{
			Vector3 position = base.BotOwner.MyHead.position;
			Vector3 position2 = enemy.EnemyPlayer.WeaponRoot.position;
			Vector3 val = position - position2;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			val = position - position2;
			if (!Physics.Raycast(position2, normalized, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
			{
				float gainSightModif = 0.66f;
				float dazzleModif = ((realDistance < MaxDazzleRange) ? GetDazzleModifier(enemy) : 1f);
				ApplyDazzle(dazzleModif, gainSightModif);
			}
			return true;
		}
		return false;
	}

	public void ApplyDazzle(float dazzleModif, float gainSightModif)
	{
		Modifiers.Modifiers.PrecicingSpeedCoef = Mathf.Clamp(dazzleModif, 1f, 5f) * Effectiveness;
		Modifiers.Modifiers.AccuratySpeedCoef = Mathf.Clamp(dazzleModif, 1f, 5f) * Effectiveness;
		Modifiers.Modifiers.GainSightCoef = gainSightModif;
		Modifiers.Modifiers.ScatteringCoef = Mathf.Clamp(dazzleModif, 1f, 5f) * Effectiveness * 3f;
		Modifiers.Modifiers.PriorityScatteringCoef = Mathf.Clamp(dazzleModif, 1f, 2.5f) * Effectiveness;
		base.BotOwner.Settings.Current.Apply(Modifiers.Modifiers, 0.1f);
	}

	private bool FlashlightVisionCheck(IPlayer person)
	{
		float num = 0.9770526f;
		return EnemyLookAtMe(person, num);
	}

	private bool LaserVisionCheck(IPlayer person)
	{
		float num = 0.99f;
		return EnemyLookAtMe(person, num);
	}

	private bool EnemyLookAtMe(IPlayer person, float num)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = base.BotOwner.MyHead.position;
		Vector3 position2 = person.WeaponRoot.position;
		return Vector.IsAngLessNormalized(Vector.NormalizeFastSelf(position - position2), person.LookDirection, num);
	}

	private float GetDazzleModifier(Enemy enemy)
	{
		float realDistance = enemy.RealDistance;
		float maxDazzleRange = MaxDazzleRange;
		float num = maxDazzleRange / 2f;
		float num2 = maxDazzleRange - num;
		float num3 = enemy.RealDistance - num2;
		float num4 = num3 / num2;
		float num5 = Mathf.InverseLerp(1f, 2f, num4);
		if (base.BotOwner.NightVision.UsingNow && (enemy.EnemyPlayerComponent.Flashlight.WhiteLight || enemy.EnemyPlayerComponent.Flashlight.Laser))
		{
			num5 *= 1.5f;
		}
		return num5;
	}
}
