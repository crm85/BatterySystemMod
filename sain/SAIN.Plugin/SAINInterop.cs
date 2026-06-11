using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Bootstrap;
using EFT;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Plugin;

internal static class SAINInterop
{
	private static bool _SAINLoadedChecked;

	private static bool _SAINInteropInited;

	private static bool _IsSAINLoaded;

	private static Type _SAINExternalType;

	private static MethodInfo _ExtractBotMethod;

	private static MethodInfo _SetExfilForBotMethod;

	private static MethodInfo _IsPathTowardEnemyMethod;

	private static MethodInfo _TimeSinceSenseEnemyMethod;

	private static MethodInfo _CanBotQuestMethod;

	private static MethodInfo _GetExtractedBotsMethod;

	private static MethodInfo _GetExtractionInfosMethod;

	private static MethodInfo _IgnoreHearingMethod;

	private static MethodInfo _GetPersonalityMethod;

	public static bool IsSAINLoaded()
	{
		if (!_SAINLoadedChecked)
		{
			_SAINLoadedChecked = true;
			_IsSAINLoaded = Chainloader.PluginInfos.ContainsKey("me.sol.sain");
		}
		return _IsSAINLoaded;
	}

	public static bool Init()
	{
		if (!IsSAINLoaded())
		{
			return false;
		}
		if (!_SAINInteropInited)
		{
			_SAINInteropInited = true;
			_SAINExternalType = Type.GetType("SAIN.Plugin.External, SAIN");
			if (_SAINExternalType != null)
			{
				_ExtractBotMethod = AccessTools.Method(_SAINExternalType, "ExtractBot", (Type[])null, (Type[])null);
				_SetExfilForBotMethod = AccessTools.Method(_SAINExternalType, "TrySetExfilForBot", (Type[])null, (Type[])null);
				_IsPathTowardEnemyMethod = AccessTools.Method(_SAINExternalType, "IsPathTowardEnemy", (Type[])null, (Type[])null);
				_TimeSinceSenseEnemyMethod = AccessTools.Method(_SAINExternalType, "TimeSinceSenseEnemy", (Type[])null, (Type[])null);
				_CanBotQuestMethod = AccessTools.Method(_SAINExternalType, "CanBotQuest", (Type[])null, (Type[])null);
				_GetExtractedBotsMethod = AccessTools.Method(_SAINExternalType, "GetExtractedBots", (Type[])null, (Type[])null);
				_GetExtractionInfosMethod = AccessTools.Method(_SAINExternalType, "GetExtractionInfos", (Type[])null, (Type[])null);
				_IgnoreHearingMethod = AccessTools.Method(_SAINExternalType, "IgnoreHearing", (Type[])null, (Type[])null);
				_GetPersonalityMethod = AccessTools.Method(_SAINExternalType, "GetPersonality", (Type[])null, (Type[])null);
			}
		}
		return _SAINExternalType != null;
	}

	public static bool IgnoreHearing(BotOwner botOwner, bool value, bool ignoreUnderFire, float duration = 0f)
	{
		if ((Object)(object)botOwner == (Object)null)
		{
			return false;
		}
		if (!Init())
		{
			return false;
		}
		if (_IgnoreHearingMethod == null)
		{
			return false;
		}
		return (bool)_IgnoreHearingMethod.Invoke(null, new object[4] { botOwner, value, ignoreUnderFire, duration });
	}

	public static string GetPersonality(BotOwner botOwner)
	{
		string empty = string.Empty;
		if ((Object)(object)botOwner == (Object)null)
		{
			return empty;
		}
		if (!Init())
		{
			return empty;
		}
		if (_GetPersonalityMethod == null)
		{
			return empty;
		}
		return (string)_GetPersonalityMethod.Invoke(null, new object[1] { botOwner });
	}

	public static bool GetExtractedBots(List<string> list)
	{
		if (list == null)
		{
			return false;
		}
		if (!Init())
		{
			return false;
		}
		if (_GetExtractedBotsMethod == null)
		{
			return false;
		}
		_GetExtractedBotsMethod.Invoke(null, new object[1] { list });
		return true;
	}

	public static bool GetExtractedBots(List<ExtractionInfo> list)
	{
		if (list == null)
		{
			return false;
		}
		if (!Init())
		{
			return false;
		}
		if (_GetExtractionInfosMethod == null)
		{
			return false;
		}
		_GetExtractionInfosMethod.Invoke(null, new object[1] { list });
		return true;
	}

	public static bool TryExtractBot(BotOwner botOwner)
	{
		if (!Init())
		{
			return false;
		}
		if (_ExtractBotMethod == null)
		{
			return false;
		}
		return (bool)_ExtractBotMethod.Invoke(null, new object[1] { botOwner });
	}

	public static bool TrySetExfilForBot(BotOwner botOwner)
	{
		if (!Init())
		{
			return false;
		}
		if (_SetExfilForBotMethod == null)
		{
			return false;
		}
		return (bool)_SetExfilForBotMethod.Invoke(null, new object[1] { botOwner });
	}

	public static bool IsPathTowardEnemy(NavMeshPath path, BotOwner botOwner, float ratioSameOverAll = 0.25f, float sqrDistCheck = 0.05f)
	{
		if (!Init())
		{
			return false;
		}
		if (_IsPathTowardEnemyMethod == null)
		{
			return false;
		}
		return (bool)_IsPathTowardEnemyMethod.Invoke(null, new object[4] { path, botOwner, ratioSameOverAll, sqrDistCheck });
	}

	public static bool CanBotQuest(BotOwner botOwner, Vector3 questPosition, float dotThreshold = 0.33f)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!Init())
		{
			return false;
		}
		if (_CanBotQuestMethod == null)
		{
			return false;
		}
		return (bool)_CanBotQuestMethod.Invoke(null, new object[3] { botOwner, questPosition, dotThreshold });
	}

	public static float TimeSinceSenseEnemy(BotOwner botOwner)
	{
		if (!Init())
		{
			return float.MaxValue;
		}
		if (_TimeSinceSenseEnemyMethod == null)
		{
			return float.MaxValue;
		}
		return (float)_TimeSinceSenseEnemyMethod.Invoke(null, new object[1] { botOwner });
	}
}
