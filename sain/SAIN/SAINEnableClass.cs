using System.Collections.Generic;
using EFT;
using SAIN.Components;
using SAIN.Components.BotController;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using UnityEngine;

namespace SAIN;

public class SAINEnableClass
{
	private static readonly HashSet<string> ExcludedBots;

	private static readonly HashSet<string> EnabledBots;

	private static VanillaBotSettings SAINEnabled => SAINPlugin.LoadedPreset.GlobalSettings.General.VanillaBots;

	static SAINEnableClass()
	{
		ExcludedBots = new HashSet<string>();
		EnabledBots = new HashSet<string>();
		GameWorld.OnDispose += Clear;
	}

	public static bool IsSAINDisabledForBot(BotOwner botOwner)
	{
		if ((Object)(object)botOwner == (Object)null)
		{
			return true;
		}
		Player getPlayer = botOwner.GetPlayer;
		if ((Object)(object)getPlayer == (Object)null)
		{
			return true;
		}
		string profileId = getPlayer.ProfileId;
		if (ExcludedBots.Contains(profileId))
		{
			return true;
		}
		if (EnabledBots.Contains(profileId))
		{
			return false;
		}
		Profile profile = botOwner.Profile;
		object obj;
		if (profile == null)
		{
			obj = null;
		}
		else
		{
			InfoClass info = profile.Info;
			obj = ((info != null) ? info.Settings : null);
		}
		ProfileInfoSettingsClass val = (ProfileInfoSettingsClass)obj;
		if (val == null)
		{
			return true;
		}
		getPlayer.OnIPlayerDeadOrUnspawn += ClearBot;
		if (IsBotExcluded(botOwner))
		{
			ExcludedBots.Add(profileId);
			Logger.LogDebug("Added Excluded Bot [" + getPlayer.Profile.Nickname + "," + profileId + "]");
			return true;
		}
		EnabledBots.Add(profileId);
		Logger.LogDebug("Added Enabled Bot [" + getPlayer.Profile.Nickname + "," + profileId + "]");
		return false;
	}

	public static bool IsSAINDisabledForBot(IPlayer iPlayer)
	{
		if (iPlayer == null || !iPlayer.IsAI)
		{
			return true;
		}
		IAIData aIData = iPlayer.AIData;
		BotOwner val = ((aIData != null) ? aIData.BotOwner : null);
		if ((Object)(object)val == (Object)null)
		{
			return true;
		}
		string profileId = iPlayer.ProfileId;
		if (ExcludedBots.Contains(profileId))
		{
			return true;
		}
		if (EnabledBots.Contains(profileId))
		{
			return false;
		}
		Profile profile = iPlayer.Profile;
		object obj;
		if (profile == null)
		{
			obj = null;
		}
		else
		{
			InfoClass info = profile.Info;
			obj = ((info != null) ? info.Settings : null);
		}
		ProfileInfoSettingsClass val2 = (ProfileInfoSettingsClass)obj;
		if (val2 == null)
		{
			return true;
		}
		val.GetPlayer.OnIPlayerDeadOrUnspawn += ClearBot;
		if (IsBotExcluded(val))
		{
			ExcludedBots.Add(profileId);
			return true;
		}
		EnabledBots.Add(profileId);
		return false;
	}

	private static void Clear()
	{
		ExcludedBots.Clear();
		EnabledBots.Clear();
	}

	private static void ClearBot(IPlayer player)
	{
		if (player != null)
		{
			player.OnIPlayerDeadOrUnspawn -= ClearBot;
			string profileId = player.ProfileId;
			ExcludedBots.Remove(profileId);
			EnabledBots.Remove(profileId);
		}
	}

	public static bool IsBotExcluded(BotOwner botOwner)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Profile profile = botOwner.Profile;
		object obj;
		if (profile == null)
		{
			obj = null;
		}
		else
		{
			InfoClass info = profile.Info;
			obj = ((info != null) ? info.Settings : null);
		}
		ProfileInfoSettingsClass val = (ProfileInfoSettingsClass)obj;
		if (val == null)
		{
			return true;
		}
		WildSpawnType role = val.Role;
		if (BotSpawnController.StrictExclusionList.Contains(role))
		{
			return true;
		}
		if (IsAlwaysEnabled(role, botOwner))
		{
			return false;
		}
		return ShallExludeByWildSpawnType(role, botOwner);
	}

	public static bool ShallExludeByWildSpawnType(WildSpawnType wildSpawnType, BotOwner botOwner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return ExcludeOthers(wildSpawnType) || ExcludeScav(wildSpawnType, botOwner) || ExcludeBoss(wildSpawnType) || ExcludeFollower(wildSpawnType) || ExcludeGoons(wildSpawnType);
	}

	private static bool IsAlwaysEnabled(WildSpawnType wildSpawnType, BotOwner botOwner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		int result;
		if (!EnumValues.WildSpawn.IsPMC(wildSpawnType))
		{
			BotManagerComponent instance = BotManagerComponent.Instance;
			result = ((instance != null && instance.Bots?.ContainsKey(botOwner.ProfileId) == true) ? 1 : 0);
		}
		else
		{
			result = 1;
		}
		return (byte)result != 0;
	}

	private static bool ExcludeBoss(WildSpawnType wildSpawnType)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return SAINEnabled.VanillaBosses && !EnumValues.WildSpawn.IsGoons(wildSpawnType) && EnumValues.WildSpawn.IsBoss(wildSpawnType);
	}

	private static bool ExcludeGoons(WildSpawnType wildSpawnType)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return SAINEnabled.VanillaGoons && EnumValues.WildSpawn.IsGoons(wildSpawnType);
	}

	private static bool ExcludeFollower(WildSpawnType wildSpawnType)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return SAINEnabled.VanillaFollowers && !EnumValues.WildSpawn.IsGoons(wildSpawnType) && EnumValues.WildSpawn.IsFollower(wildSpawnType);
	}

	private static bool ExcludeScav(WildSpawnType wildSpawnType, BotOwner botOwner)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return SAINEnabled.VanillaScavs && EnumValues.WildSpawn.IsScav(wildSpawnType) && !IsPlayerScav(botOwner.Profile);
	}

	private static bool ExcludeOthers(WildSpawnType wildSpawnType)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		if (SAINEnabled.VanillaCultists && EnumValues.WildSpawn.IsCultist(wildSpawnType))
		{
			return true;
		}
		if (SAINEnabled.VanillaRogues && (int)wildSpawnType == 24)
		{
			return true;
		}
		if (SAINEnabled.VanillaBloodHounds && ((int)wildSpawnType == 34 || (int)wildSpawnType == 35))
		{
			return true;
		}
		return false;
	}

	public static bool IsPlayerScav(Profile profile)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		if (profile.Info.Nickname.Contains(" ("))
		{
			return true;
		}
		return (int)profile.Info.Settings.Role == 1 && !string.IsNullOrEmpty(profile.Info.MainProfileNickname);
	}

	public static bool IsBotInCombat(IPlayer player)
	{
		BotManagerComponent instance = BotManagerComponent.Instance;
		return instance != null && instance.BotSpawnController?.GetSAIN(player.ProfileId)?.SAINLayersActive == true;
	}

	public static bool GetSAIN(BotOwner botOwner, out BotComponent sain)
	{
		sain = null;
		if (IsSAINDisabledForBot(botOwner))
		{
			return false;
		}
		if ((Object)(object)BotManagerComponent.Instance == (Object)null)
		{
			return false;
		}
		return BotManagerComponent.Instance.GetSAIN(botOwner, out sain);
	}

	public static bool GetSAIN(Player player, out BotComponent sain)
	{
		object botOwner;
		if (player == null)
		{
			botOwner = null;
		}
		else
		{
			IAIData aIData = player.AIData;
			botOwner = ((aIData != null) ? aIData.BotOwner : null);
		}
		return GetSAIN((BotOwner)botOwner, out sain);
	}
}
