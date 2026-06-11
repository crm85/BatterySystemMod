using System;
using System.Reflection;
using System.Text;
using EFT;
using HarmonyLib;
using SAIN.Classes.Coverfinder;
using SAIN.Components;
using SAIN.Preset.BotSettings.SAINSettings.Categories;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Shoot.Aim;

public class AimTimePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotAimingClass), "method_7", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotAimingClass __instance, float dist, float ang, ref float __result)
	{
		if (!SAINEnableClass.GetSAIN(__instance.botOwner_0, out var sain))
		{
			return true;
		}
		__result = CalculateAim(sain, dist, ang, __instance.bool_1, __instance.bool_0, __instance.float_10);
		sain.Aim.LastAimTime = __result;
		return false;
	}

	private static float CalculateAim(BotComponent botComponent, float distance, float angle, bool moving, bool panicing, float aimDelay)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		BotOwner botOwner = botComponent.BotOwner;
		StringBuilder stringBuilder = (SAINPlugin.LoadedPreset.GlobalSettings.General.Debug.Logs.DebugAimCalculations ? new StringBuilder() : null);
		if (stringBuilder != null)
		{
			string arg = ((botOwner != null) ? ((Object)botOwner).name : null);
			WildSpawnType? obj;
			if (botOwner == null)
			{
				obj = null;
			}
			else
			{
				Profile profile = botOwner.Profile;
				if (profile == null)
				{
					obj = null;
				}
				else
				{
					InfoClass info = profile.Info;
					obj = ((info == null) ? ((WildSpawnType?)null) : info.Settings?.Role);
				}
			}
			object arg2 = obj;
			BotDifficulty? obj2;
			if (botOwner == null)
			{
				obj2 = null;
			}
			else
			{
				Profile profile2 = botOwner.Profile;
				if (profile2 == null)
				{
					obj2 = null;
				}
				else
				{
					InfoClass info2 = profile2.Info;
					obj2 = ((info2 == null) ? ((BotDifficulty?)null) : info2.Settings?.BotDifficulty);
				}
			}
			stringBuilder.AppendLine($"Aim Time Calculation for [{arg} : {arg2} : {obj2}]");
		}
		SAINAimingSettings aiming = botComponent.Info.FileSettings.Aiming;
		BotSettingsComponents fileSettings = botOwner.Settings.FileSettings;
		float bOTTOM_COEF = fileSettings.Aiming.BOTTOM_COEF;
		stringBuilder?.AppendLine($"baseAimTime [{bOTTOM_COEF}]");
		bOTTOM_COEF = CalcCoverMod(bOTTOM_COEF, botOwner, botComponent, fileSettings, stringBuilder);
		BotCurvSettings curv = botOwner.Settings.Curv;
		float angleTime = CalcCurveOutput(curv.AimAngCoef, angle, aiming.AngleAimTimeMultiplier, stringBuilder, "Angle");
		float distanceTime = CalcCurveOutput(curv.AimTime2Dist, distance, aiming.DistanceAimTimeMultiplier, stringBuilder, "Distance");
		float calculatedAimTime = CalcAimTime(angleTime, distanceTime, botOwner, stringBuilder);
		calculatedAimTime = CalcPanic(panicing, calculatedAimTime, fileSettings, stringBuilder);
		float num = bOTTOM_COEF + calculatedAimTime + aimDelay;
		stringBuilder?.AppendLine($"timeToAimResult [{num}] (baseAimTime + calculatedAimTime + aimDelay)");
		num = CalcMoveModifier(moving, num, fileSettings, stringBuilder);
		BotWeaponManager weaponManager = botOwner.WeaponManager;
		int aiming2;
		if (weaponManager == null)
		{
			aiming2 = 0;
		}
		else
		{
			IFirearmHandsController shootController = weaponManager.ShootController;
			aiming2 = ((((shootController != null) ? new bool?(((IHandsController)shootController).IsAiming) : ((bool?)null)) == true) ? 1 : 0);
		}
		num = CalcADSModifier((byte)aiming2 != 0, num, stringBuilder);
		num = ClampAimTime(num, fileSettings, stringBuilder);
		num = CalcFasterCQB(distance, num, aiming, stringBuilder);
		num = CalcAttachmentMod(botComponent, num, stringBuilder);
		if (stringBuilder != null && botOwner != null)
		{
			BotMemoryClass memory = botOwner.Memory;
			bool? obj3;
			if (memory == null)
			{
				obj3 = null;
			}
			else
			{
				EnemyInfo goalEnemy = memory.GoalEnemy;
				if (goalEnemy == null)
				{
					obj3 = null;
				}
				else
				{
					IPlayer person = goalEnemy.Person;
					obj3 = ((person != null) ? new bool?(person.IsYourPlayer) : ((bool?)null));
				}
			}
			bool? flag = obj3;
			if (flag == true)
			{
				ModulePatch.Logger.LogDebug((object)stringBuilder.ToString());
			}
		}
		return num;
	}

	private static float CalcAimTime(float angleTime, float distanceTime, BotOwner botOwner, StringBuilder stringBuilder)
	{
		float currentAccuratySpeed = botOwner.Settings.Current.CurrentAccuratySpeed;
		stringBuilder?.AppendLine($"accuracySpeed [{currentAccuratySpeed}]");
		float num = angleTime * distanceTime * currentAccuratySpeed;
		stringBuilder?.AppendLine($"calculatedAimTime [{num}] (angleTime * distanceTime * accuracySpeed)");
		return num;
	}

	private static float CalcCoverMod(float baseAimTime, BotOwner botOwner, BotComponent botComponent, BotSettingsComponents fileSettings, StringBuilder stringBuilder)
	{
		CoverPoint coverPoint = botComponent?.Cover.CoverInUse;
		if (botOwner.Memory.IsInCover || (coverPoint != null && coverPoint.BotInThisCover))
		{
			baseAimTime *= fileSettings.Aiming.COEF_FROM_COVER;
			stringBuilder?.AppendLine($"In Cover: [{baseAimTime}] : COEF_FROM_COVER [{fileSettings.Aiming.COEF_FROM_COVER}]");
		}
		return baseAimTime;
	}

	private static float CalcCurveOutput(AnimationCurve aimCurve, float input, float modifier, StringBuilder stringBuilder, string curveType)
	{
		float num = aimCurve.Evaluate(input);
		num *= modifier;
		stringBuilder?.AppendLine($"{curveType} Curve Output [{num}] : input [{input}] : Multiplier: [{modifier}]");
		return num;
	}

	private static float CalcMoveModifier(bool moving, float timeToAimResult, BotSettingsComponents fileSettings, StringBuilder stringBuilder)
	{
		if (moving)
		{
			timeToAimResult *= fileSettings.Aiming.COEF_IF_MOVE;
			stringBuilder?.AppendLine($"Moving [{timeToAimResult}] : Moving Coef [{fileSettings.Aiming.COEF_IF_MOVE}]");
		}
		return timeToAimResult;
	}

	private static float CalcADSModifier(bool aiming, float timeToAimResult, StringBuilder stringBuilder)
	{
		if (aiming)
		{
			float aimDownSightsAimTimeMultiplier = SAINPlugin.LoadedPreset.GlobalSettings.Aiming.AimDownSightsAimTimeMultiplier;
			timeToAimResult *= aimDownSightsAimTimeMultiplier;
			stringBuilder?.AppendLine($"Aiming Down Sights [{timeToAimResult}] : ADS Multiplier [{aimDownSightsAimTimeMultiplier}]");
		}
		return timeToAimResult;
	}

	private static float ClampAimTime(float timeToAimResult, BotSettingsComponents fileSettings, StringBuilder stringBuilder)
	{
		float num = Mathf.Clamp(timeToAimResult, 0f, fileSettings.Aiming.MAX_AIM_TIME);
		if (num != timeToAimResult)
		{
			stringBuilder?.AppendLine($"Clamped Aim Time [{num}] : MAX_AIM_TIME [{fileSettings.Aiming.MAX_AIM_TIME}]");
		}
		return num;
	}

	private static float CalcPanic(bool panicing, float calculatedAimTime, BotSettingsComponents fileSettings, StringBuilder stringBuilder)
	{
		if (panicing)
		{
			calculatedAimTime *= fileSettings.Aiming.PANIC_COEF;
			stringBuilder?.AppendLine($"Panicing [{calculatedAimTime}] : Panic Coef [{fileSettings.Aiming.PANIC_COEF}]");
		}
		return calculatedAimTime;
	}

	private static float CalcFasterCQB(float distance, float aimTimeResult, SAINAimingSettings aimSettings, StringBuilder stringBuilder)
	{
		if (!SAINPlugin.LoadedPreset.GlobalSettings.Aiming.FasterCQBReactionsGlobal)
		{
			return aimTimeResult;
		}
		if (aimSettings != null && aimSettings.FasterCQBReactions && distance <= aimSettings.FasterCQBReactionsDistance)
		{
			float num = distance / aimSettings.FasterCQBReactionsDistance;
			float num2 = aimTimeResult * num;
			num2 = Mathf.Clamp(num2, aimSettings.FasterCQBReactionsMinimum, aimTimeResult);
			stringBuilder?.AppendLine($"Faster CQB Aim Time: Result [{num2}] : Original [{aimTimeResult}] : At Distance [{distance}] with maxDist [{aimSettings.FasterCQBReactionsDistance}]");
			return num2;
		}
		return aimTimeResult;
	}

	private static float CalcAttachmentMod(BotComponent bot, float aimTimeResult, StringBuilder stringBuilder)
	{
		Enemy enemy = bot?.Enemy;
		if (enemy != null)
		{
			float aimAndScatterMultiplier = enemy.Aim.AimAndScatterMultiplier;
			stringBuilder?.AppendLine($"Bot Attachment Mod: Result [{aimTimeResult / aimAndScatterMultiplier}] : Original [{aimTimeResult}] : Modifier [{aimAndScatterMultiplier}]");
			aimTimeResult /= aimAndScatterMultiplier;
		}
		return aimTimeResult;
	}
}
