using System;
using EFT;
using SAIN.Components;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.Helpers;

public class Shoot
{
	public static float FullAutoBurstLength(BotOwner BotOwner, float distance)
	{
		BotComponent component = ((Component)BotOwner).GetComponent<BotComponent>();
		if ((Object)(object)component == (Object)null)
		{
			return 0.1f;
		}
		if (component.IsCheater)
		{
			return 1f;
		}
		if (component.ManualShoot.Reason != EShootReason.None && component.Info.WeaponInfo.EWeaponClass == EWeaponClass.machinegun)
		{
			return 0.75f;
		}
		float num = 1f - Mathf.Clamp(distance, 0f, 30f) / 30f;
		num /= component.Info.WeaponInfo.FinalModifier;
		num *= component.Info.FileSettings.Shoot.BurstMulti;
		num = Mathf.Clamp(num, 0.001f, 1f);
		if (distance > 30f)
		{
			num = 0.001f;
		}
		else if (distance < 5f)
		{
			num = 1f;
		}
		return num;
	}

	public static float FullAutoTimePerShot(int bFirerate)
	{
		float num = bFirerate / 60;
		return 1f / num;
	}

	public static float InverseScaleWithLogisticFunction(float originalValue, float k, float x0 = 20f)
	{
		float num = 1f - 1f / (1f + Mathf.Exp(k * (originalValue - x0)));
		return (float)Math.Round(num, 3);
	}
}
