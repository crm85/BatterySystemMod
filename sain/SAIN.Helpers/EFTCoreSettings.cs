using System;

namespace SAIN.Helpers;

public class EFTCoreSettings
{
	private static CoreOverrides _overrides;

	public GClass598 Core;

	static EFTCoreSettings()
	{
		if (!JsonUtility.Load.LoadObject<CoreOverrides>(out _overrides, "CoreOverrides"))
		{
			_overrides = new CoreOverrides();
			JsonUtility.SaveObjectToJson(_overrides, "CoreOverrides");
		}
	}

	public static void UpdateCoreSettings()
	{
		try
		{
			CoreBotSettingsClass core = GClass598.Core;
			if (_overrides == null)
			{
				_overrides = new CoreOverrides();
			}
			core.SCAV_GROUPS_TOGETHER = _overrides.SCAV_GROUPS_TOGETHER;
			core.DIST_NOT_TO_GROUP = _overrides.DIST_NOT_TO_GROUP;
			core.DIST_NOT_TO_GROUP_SQR = core.DIST_NOT_TO_GROUP.Sqr();
			core.CAN_SHOOT_TO_HEAD = _overrides.CAN_SHOOT_TO_HEAD;
			core.SOUND_DOOR_OPEN_METERS = _overrides.SOUND_DOOR_OPEN_METERS;
			core.SOUND_DOOR_BREACH_METERS = _overrides.SOUND_DOOR_BREACH_METERS;
			core.JUMP_SPREAD_DIST = _overrides.JUMP_SPREAD_DIST;
			core.BASE_WALK_SPEREAD2 = _overrides.BASE_WALK_SPEREAD2;
			core.GRENADE_PRECISION = _overrides.GRENADE_PRECISION;
			core.PRONE_POSE = _overrides.PRONE_POSE;
			core.MOVE_COEF = _overrides.MOVE_COEF;
			core.LOWER_POSE = _overrides.LOWER_POSE;
			core.MAX_POSE = _overrides.MAX_POSE;
			core.FLARE_POWER = _overrides.FLARE_POWER;
			core.FLARE_TIME = _overrides.FLARE_TIME;
			core.SHOOT_TO_CHANGE_RND_PART_DELTA = _overrides.SHOOT_TO_CHANGE_RND_PART_DELTA;
			ModDetection.UpdateArmorClassCoef();
		}
		catch (Exception data)
		{
			Logger.LogError(data);
		}
	}

	public static void UpdateArmorClassCoef(float coef)
	{
		GClass598.Core.ARMOR_CLASS_COEF = coef;
	}
}
