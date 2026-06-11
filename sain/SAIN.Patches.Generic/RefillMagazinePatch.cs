using System;
using System.Collections.Generic;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic;

public class RefillMagazinePatch : ModulePatch
{
	private static readonly List<AmmoItemClass> _preallocatedAmmoList = new List<AmmoItemClass>(100);

	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotReload), "method_2", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotReload __instance, BotOwner ___botOwner_0, Weapon weapon, MagazineItemClass foundMag)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		if (!___botOwner_0.GetPlayer.HealthController.IsAlive)
		{
			return false;
		}
		AmmoItemClass val = method_3(weapon, foundMag, ___botOwner_0.GetPlayer);
		if (val == null)
		{
			return false;
		}
		GStruct454 val2 = ((CompoundItem)foundMag).Apply((TraderControllerClass)(object)((GClass419)__instance).botOwner_0.GetPlayer.InventoryController, (Item)(object)val, ((Item)val).StackObjectsCount, true);
		if (((GStruct454)(ref val2)).Failed)
		{
			ModulePatch.Logger.LogDebug((object)("failed to fill mag [" + ((object)((GStruct454)(ref val2)).Error)?.ToString() + "]"));
		}
		((TraderControllerClass)((GClass419)__instance).botOwner_0.GetPlayer.InventoryController).TryRunNetworkTransaction(val2, new Callback(BotReload.smethod_0));
		return false;
	}

	private static AmmoItemClass method_3(Weapon weapon, MagazineItemClass foundMag, Player player)
	{
		Slot val = (weapon.HasChambers ? weapon.Chambers[0] : null);
		_preallocatedAmmoList.Clear();
		player.InventoryController.GetAcceptableItemsNonAlloc<AmmoItemClass>(BotReload._availableEquipmentSlots, (IList<AmmoItemClass>)_preallocatedAmmoList, (Predicate<AmmoItemClass>)null, (Predicate<GClass3050>)null);
		AmmoItemClass val2 = null;
		foreach (AmmoItemClass preallocatedAmmo in _preallocatedAmmoList)
		{
			bool flag = val != null && GClass2928.CanAccept((IContainer)(object)val, (Item)(object)preallocatedAmmo);
			bool flag2 = GClass2928.CheckItemFilter(foundMag.Cartridges.Filters, (Item)(object)preallocatedAmmo) && (val2 == null || ((Item)val2).StackObjectsCount < ((Item)preallocatedAmmo).StackObjectsCount);
			if (flag || flag2)
			{
				ModulePatch.Logger.LogDebug((object)$"Ammo [{((Item)preallocatedAmmo).Name}] Slot Accepts? [{flag}] filterAccepts? [{flag2}]");
				val2 = preallocatedAmmo;
			}
		}
		ModulePatch.Logger.LogDebug((object)$"Ammo [{((val2 != null) ? ((Item)val2).Name : null)}] Selected out of {_preallocatedAmmoList.Count}");
		_preallocatedAmmoList.Clear();
		return val2;
	}
}
