using System;
using System.Reflection;
using BepInEx.Bootstrap;
using EFT;
using EFT.Interactive;
using HarmonyLib;

namespace LootingBots;

internal static class LootingBotsInterop
{
	private static bool _LootingBotsLoadedChecked;

	private static bool _LootingBotsInteropInited;

	private static bool _IsLootingBotsLoaded;

	private static Type _LootingBotsExternalType;

	private static MethodInfo _ForceBotToScanLootMethod;

	private static MethodInfo _PreventBotFromLootingMethod;

	private static MethodInfo _CheckIfInventoryFullMethod;

	private static MethodInfo _GetNetLootValueMethod;

	private static MethodInfo _GetItemPriceMethod;

	public static bool IsLootingBotsLoaded()
	{
		if (!_LootingBotsLoadedChecked)
		{
			_LootingBotsLoadedChecked = true;
			_IsLootingBotsLoaded = Chainloader.PluginInfos.ContainsKey("me.skwizzy.lootingbots");
		}
		return _IsLootingBotsLoaded;
	}

	public static bool Init()
	{
		if (!IsLootingBotsLoaded())
		{
			return false;
		}
		if (!_LootingBotsInteropInited)
		{
			_LootingBotsInteropInited = true;
			_LootingBotsExternalType = Type.GetType("LootingBots.External, skwizzy.LootingBots");
			if (_LootingBotsExternalType != null)
			{
				_ForceBotToScanLootMethod = AccessTools.Method(_LootingBotsExternalType, "ForceBotToScanLoot", (Type[])null, (Type[])null);
				_PreventBotFromLootingMethod = AccessTools.Method(_LootingBotsExternalType, "PreventBotFromLooting", (Type[])null, (Type[])null);
				_CheckIfInventoryFullMethod = AccessTools.Method(_LootingBotsExternalType, "CheckIfInventoryFull", (Type[])null, (Type[])null);
				_GetNetLootValueMethod = AccessTools.Method(_LootingBotsExternalType, "GetNetLootValue", (Type[])null, (Type[])null);
				_GetItemPriceMethod = AccessTools.Method(_LootingBotsExternalType, "GetItemPrice", (Type[])null, (Type[])null);
			}
		}
		return _LootingBotsExternalType != null;
	}

	public static bool TryForceBotToScanLoot(BotOwner botOwner)
	{
		if (!Init())
		{
			return false;
		}
		if (_ForceBotToScanLootMethod == null)
		{
			return false;
		}
		return (bool)_ForceBotToScanLootMethod.Invoke(null, new object[1] { botOwner });
	}

	public static bool TryPreventBotFromLooting(BotOwner botOwner, float duration)
	{
		if (!Init())
		{
			return false;
		}
		if (_PreventBotFromLootingMethod == null)
		{
			return false;
		}
		return (bool)_PreventBotFromLootingMethod.Invoke(null, new object[2] { botOwner, duration });
	}

	public static bool CheckIfInventoryFull(BotOwner botOwner)
	{
		if (!Init())
		{
			return false;
		}
		if (_CheckIfInventoryFullMethod == null)
		{
			return false;
		}
		return (bool)_CheckIfInventoryFullMethod.Invoke(null, new object[1] { botOwner });
	}

	public static float GetNetLootValue(BotOwner botOwner)
	{
		if (!Init())
		{
			return 0f;
		}
		if (_GetNetLootValueMethod == null)
		{
			return 0f;
		}
		return (float)_GetNetLootValueMethod.Invoke(null, new object[1] { botOwner });
	}

	public static float GetItemPrice(LootItem item)
	{
		if (!Init())
		{
			return 0f;
		}
		if (_GetItemPriceMethod == null)
		{
			return 0f;
		}
		return (float)_GetItemPriceMethod.Invoke(null, new object[1] { item });
	}
}
