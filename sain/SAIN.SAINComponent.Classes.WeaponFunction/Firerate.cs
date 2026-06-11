using System.Collections.Generic;
using EFT.InventoryLogic;
using SAIN.Components;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class Firerate : BotBase
{
	public float PerMeter
	{
		get
		{
			Dictionary<EWeaponClass, float> dictionary = BotBase.GlobalSettings?.Shoot?.WeaponPerMeter;
			WeaponInfoClass weaponInfoClass = base.Bot?.Info?.WeaponInfo;
			if (dictionary != null && weaponInfoClass != null)
			{
				if (dictionary.TryGetValue(weaponInfoClass.EWeaponClass, out var value))
				{
					return value;
				}
				if (dictionary.TryGetValue(EWeaponClass.Default, out value))
				{
					return value;
				}
			}
			return 80f;
		}
	}

	public Firerate(BotComponent sain)
		: base(sain)
	{
	}

	public float SemiAutoROF()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		WeaponInfoClass weaponInfo = base.Bot.Info.WeaponInfo;
		if (weaponInfo == null)
		{
			return 1f;
		}
		if (base.Bot.IsCheater)
		{
			return 0f;
		}
		float num = 0.1f;
		float num2 = 4f;
		Vector3 val = base.BotOwner.AimingManager.CurrentAiming.RealTargetPoint - base.BotOwner.WeaponRoot.position;
		float magnitude = ((Vector3)(ref val)).magnitude;
		float num3 = magnitude / (PerMeter / weaponInfo.FinalModifier);
		float num4 = Mathf.Clamp(num3, num, num2);
		if (weaponInfo.IsFireModeSet((EFireMode)0))
		{
			num4 = Mathf.Clamp(num4 * 0.25f, 0.001f, 1f);
		}
		if (weaponInfo.IsFireModeSet((EFireMode)3))
		{
			num4 = Mathf.Clamp(num4, 0.1f, 3f);
		}
		num4 /= base.Bot.Info.FileSettings.Shoot.FireratMulti;
		float num5 = num4 * Random.Range(0.85f, 1.15f);
		return Mathf.Round(num5 * 100f) / 100f;
	}
}
